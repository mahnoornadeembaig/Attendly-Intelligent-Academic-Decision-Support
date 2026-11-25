using System.Data;
using System.Globalization;

namespace AMS.Classes
{
    internal sealed class AttendanceBehaviorAnalyzer
    {
        internal sealed class Analysis
        {
            public string WeekdayName { get; init; } = "No repeated pattern";
            public double WeekdayAbsenceRate { get; init; }
            public string CourseName { get; init; } = "No repeated pattern";
            public double CourseAbsenceRate { get; init; }
            public string TrendText { get; init; } = "Not enough history";
            public bool TrendDeclining { get; init; }
            public int TrainingRecords { get; init; }
            public double Confidence { get; init; }
        }

        private sealed class Record
        {
            public DayOfWeek Weekday { get; init; }
            public DateTime Date { get; init; }
            public string Course { get; init; } = string.Empty;
            public bool Absent { get; init; }
        }

        public static Analysis Analyze(DataTable rows)
        {
            var records = new List<Record>();
            foreach (DataRow row in rows.Rows)
            {
                if (!DateTime.TryParse(
                        Convert.ToString(row["Date"]),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowWhiteSpaces,
                        out DateTime date) &&
                    !DateTime.TryParse(
                        Convert.ToString(row["Date"]),
                        CultureInfo.CurrentCulture,
                        DateTimeStyles.AllowWhiteSpaces,
                        out date))
                {
                    continue;
                }

                string status = Convert.ToString(row["Status"]) ?? string.Empty;
                bool absent = status.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("Absent", StringComparison.OrdinalIgnoreCase);
                records.Add(new Record
                {
                    Weekday = date.DayOfWeek,
                    Date = date,
                    Course = Convert.ToString(row["Course"]) ?? "Unknown course",
                    Absent = absent
                });
            }

            if (records.Count == 0)
            {
                return new Analysis();
            }

            var weekdayTotals = records
                .GroupBy(record => record.Weekday)
                .Select(group => new
                {
                    Weekday = group.Key,
                    Total = group.Count(),
                    Absences = group.Count(record => record.Absent),
                    Rate = group.Count(record => record.Absent) / (double)group.Count()
                })
                .OrderByDescending(item => item.Rate)
                .ThenByDescending(item => item.Total)
                .First();

            var courseTotals = records
                .GroupBy(record => record.Course)
                .Select(group => new
                {
                    Course = group.Key,
                    Total = group.Count(),
                    Absences = group.Count(record => record.Absent),
                    Rate = group.Count(record => record.Absent) / (double)group.Count()
                })
                .OrderByDescending(item => item.Rate)
                .ThenByDescending(item => item.Total)
                .First();

            bool trendDeclining = false;
            string trendText = "Not enough history";
            if (records.Count >= 8)
            {
                var ordered = records.OrderBy(record => record.Date).ToList();
                int segmentSize = Math.Max(3, ordered.Count / 3);
                var first = ordered.Take(segmentSize).ToList();
                var last = ordered.TakeLast(segmentSize).ToList();
                double firstRate = first.Count(record => record.Absent) / (double)first.Count;
                double lastRate = last.Count(record => record.Absent) / (double)last.Count;
                trendDeclining = lastRate > firstRate + 0.10;
                trendText = trendDeclining
                    ? $"Semester trend: absences increased {Math.Abs(lastRate - firstRate):P0}"
                    : $"Semester trend: attendance stable or improving";
            }

            double confidence = TrainAndEvaluate(records);
            return new Analysis
            {
                WeekdayName = $"{weekdayTotals.Weekday} ({weekdayTotals.Rate:P0} absence)",
                WeekdayAbsenceRate = weekdayTotals.Rate,
                CourseName = $"{courseTotals.Course} ({courseTotals.Rate:P0} absence)",
                CourseAbsenceRate = courseTotals.Rate,
                TrendText = trendText,
                TrendDeclining = trendDeclining,
                TrainingRecords = records.Count,
                Confidence = confidence
            };
        }

        private static double TrainAndEvaluate(List<Record> records)
        {
            if (records.Count < 6)
            {
                return 0.50;
            }

            var courses = records
                .Select(record => record.Course)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select((course, index) => (course, index))
                .ToDictionary(item => item.course, item => item.index, StringComparer.OrdinalIgnoreCase);
            int featureCount = 7 + courses.Count + 1;
            var weights = new double[featureCount];
            var random = new Random(2026);

            for (int epoch = 0; epoch < 500; epoch++)
            {
                foreach (var record in records.OrderBy(_ => random.Next()))
                {
                    var features = BuildFeatures(record, records, courses, featureCount);
                    double score = 0;
                    for (int i = 0; i < featureCount; i++)
                        score += weights[i] * features[i];
                    double predicted = Sigmoid(score);
                    double target = record.Absent ? 1.0 : 0.0;
                    double error = target - predicted;
                    double learningRate = 0.05;
                    for (int i = 0; i < featureCount; i++)
                        weights[i] += learningRate * error * features[i];
                }
            }

            int correct = 0;
            foreach (var record in records)
            {
                var features = BuildFeatures(record, records, courses, featureCount);
                double score = 0;
                for (int i = 0; i < featureCount; i++)
                    score += weights[i] * features[i];
                bool predictedAbsent = Sigmoid(score) >= 0.5;
                if (predictedAbsent == record.Absent)
                    correct++;
            }

            return correct / (double)records.Count;
        }

        private static double[] BuildFeatures(
            Record record,
            List<Record> records,
            Dictionary<string, int> courses,
            int featureCount)
        {
            var features = new double[featureCount];
            features[(int)record.Weekday == 0 ? 6 : (int)record.Weekday - 1] = 1;
            if (courses.TryGetValue(record.Course, out int courseIndex))
                features[7 + courseIndex] = 1;

            DateTime first = records.Min(item => item.Date);
            DateTime last = records.Max(item => item.Date);
            double span = (last - first).TotalDays;
            features[^1] = span <= 0 ? 0.5 : (record.Date - first).TotalDays / span;
            return features;
        }

        private static double Sigmoid(double value)
        {
            if (value >= 30) return 1;
            if (value <= -30) return 0;
            return 1.0 / (1.0 + Math.Exp(-value));
        }
    }
}
