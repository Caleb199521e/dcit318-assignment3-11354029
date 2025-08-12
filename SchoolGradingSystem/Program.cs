using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGrading
{
    // Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }

        public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var sr = new StreamReader(inputFilePath);
            string? line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');

                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNumber}: missing fields.");

                // Trim spaces
                var idPart = parts[0].Trim();
                var namePart = parts[1].Trim();
                var scorePart = parts[2].Trim();

                if (!int.TryParse(idPart, out var id))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: ID '{idPart}' is not a valid integer.");

                if (!int.TryParse(scorePart, out var score))
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{scorePart}' is not a valid integer.");

                students.Add(new Student(id, namePart, score));
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var sw = new StreamWriter(outputFilePath, false);
            foreach (var student in students)
            {
                sw.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            // Update these paths before running
            string inputPath = "students_input.txt";
            string outputPath = "students_report.txt";

            var processor = new StudentResultProcessor();

            try
            {
                var students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);
                Console.WriteLine($"Report written to {outputPath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score format: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
