using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : Controller
    {
        private readonly AppDbContext _context;

        public MarkController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        // Helper method to create the standard response format
        private MarkDto CreateMarkResponse(Student student, List<Marks> marks, int year)
        {
            // Group marks by semester and handle duplicates by taking the first occurrence
            var term1Marks = marks
                .Where(m => m.Semestre == 1)
                .GroupBy(m => m.subject.Name.ToLower())
                .ToDictionary(g => g.Key, g => (double)g.First().mark);

            var term2Marks = marks
                .Where(m => m.Semestre == 2)
                .GroupBy(m => m.subject.Name.ToLower())
                .ToDictionary(g => g.Key, g => (double)g.First().mark);

            var term3Marks = marks
                .Where(m => m.Semestre == 3)
                .GroupBy(m => m.subject.Name.ToLower())
                .ToDictionary(g => g.Key, g => (double)g.First().mark);

            var sem1Avg = term1Marks.Any() ? term1Marks.Values.Average() : 0;
            var sem2Avg = term2Marks.Any() ? term2Marks.Values.Average() : 0;
            var sem3Avg = term3Marks.Any() ? term3Marks.Values.Average() : 0;
            var finalAverage = (sem1Avg + sem2Avg + sem3Avg) / 3;

            return new MarkDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentId = student.Id,
                Year = year,
                Term1 = new TermMarksDto
                {
                    Mathematics = term1Marks.GetValueOrDefault("mathematics", 0),
                    Science = term1Marks.GetValueOrDefault("science", 0),
                    English = term1Marks.GetValueOrDefault("english", 0),
                    French = term1Marks.GetValueOrDefault("french", 0),
                    History = term1Marks.GetValueOrDefault("history", 0),
                    Geography = term1Marks.GetValueOrDefault("geography", 0),
                    Physical = term1Marks.GetValueOrDefault("physical", 0)
                },
                Term2 = new TermMarksDto
                {
                    Mathematics = term2Marks.GetValueOrDefault("mathematics", 0),
                    Science = term2Marks.GetValueOrDefault("science", 0),
                    English = term2Marks.GetValueOrDefault("english", 0),
                    French = term2Marks.GetValueOrDefault("french", 0),
                    History = term2Marks.GetValueOrDefault("history", 0),
                    Geography = term2Marks.GetValueOrDefault("geography", 0),
                    Physical = term2Marks.GetValueOrDefault("physical", 0)
                },
                Term3 = new TermMarksDto
                {
                    Mathematics = term3Marks.GetValueOrDefault("mathematics", 0),
                    Science = term3Marks.GetValueOrDefault("science", 0),
                    English = term3Marks.GetValueOrDefault("english", 0),
                    French = term3Marks.GetValueOrDefault("french", 0),
                    History = term3Marks.GetValueOrDefault("history", 0),
                    Geography = term3Marks.GetValueOrDefault("geography", 0),
                    Physical = term3Marks.GetValueOrDefault("physical", 0)
                },
                FinalAverage = finalAverage,
                Remarks = marks.FirstOrDefault()?.Remarque ?? "No remarks"
            };
        }

        // Helper method to add or update a single subject mark
        private async Task AddOrUpdateSubjectMark(int studentId, string subjectName, double markValue, int semester, int year, string remarks)
        {
            if (markValue < 0) return; // Skip negative marks

            // Find or create subject
            var subject = await _context.subjects
                .FirstOrDefaultAsync(s => s.Name.ToLower() == subjectName.ToLower());

            if (subject == null)
            {
                subject = new Subject { Name = subjectName };
                _context.subjects.Add(subject);
                await _context.SaveChangesAsync();
            }

            // Check if mark exists
            var existingMark = await _context.marks
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.SubjectId == subject.id &&
                    m.Semestre == semester &&
                    m.Year == year);

            if (existingMark != null)
            {
                // Update existing mark
                existingMark.mark = markValue;
                existingMark.Remarque = remarks;
            }
            else
            {
                // Create new mark
                var newMark = new Marks
                {
                    StudentId = studentId,
                    SubjectId = subject.id,
                    mark = markValue,
                    Semestre = semester,
                    Year = year,
                    Remarque = remarks
                };
                _context.marks.Add(newMark);
            }
        }

        [HttpPost("add-or-update-mark")]
        public async Task<ActionResult<MarkDto>> AddOrUpdateMark([FromBody] MarkUpdateRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Find or create subject
            var subject = await _context.subjects
                .FirstOrDefaultAsync(s => s.Name.ToLower() == request.SubjectName.ToLower());

            if (subject == null)
            {
                subject = new Subject { Name = request.SubjectName };
                _context.subjects.Add(subject);
                await _context.SaveChangesAsync();
            }

            // Check if mark exists
            var existingMark = await _context.marks
                .FirstOrDefaultAsync(m =>
                    m.StudentId == student.Id &&
                    m.SubjectId == subject.id &&
                    m.Semestre == request.Semester &&
                    m.Year == request.Year);

            if (existingMark != null)
            {
                // Update existing mark
                existingMark.mark = request.Mark;
                existingMark.Remarque = request.Remarks;
            }
            else
            {
                // Create new mark
                var newMark = new Marks
                {
                    StudentId = student.Id,
                    SubjectId = subject.id,
                    mark = request.Mark,
                    Semestre = request.Semester,
                    Year = request.Year,
                    Remarque = request.Remarks
                };
                _context.marks.Add(newMark);
            }

            await _context.SaveChangesAsync();

            // Return the complete updated mark report
            var updatedMarks = await _context.marks
                .Where(m => m.StudentId == student.Id && m.Year == request.Year)
                .Include(m => m.subject)
                .ToListAsync();

            return Ok(CreateMarkResponse(student, updatedMarks, request.Year));
        }

        [HttpPost("add-full-mark")]
        public async Task<ActionResult<MarkDto>> AddFullMark([FromBody] MarkFullUpdateRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Process marks for each term
            if (request.Term1 != null)
            {
                await AddOrUpdateSubjectMark(student.Id, "mathematics", request.Term1.Mathematics, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "science", request.Term1.Science, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "english", request.Term1.English, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "french", request.Term1.French, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "history", request.Term1.History, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "geography", request.Term1.Geography, 1, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "physical", request.Term1.Physical, 1, request.Year, request.Remarks);
            }

            if (request.Term2 != null)
            {
                await AddOrUpdateSubjectMark(student.Id, "mathematics", request.Term2.Mathematics, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "science", request.Term2.Science, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "english", request.Term2.English, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "french", request.Term2.French, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "history", request.Term2.History, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "geography", request.Term2.Geography, 2, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "physical", request.Term2.Physical, 2, request.Year, request.Remarks);
            }

            if (request.Term3 != null)
            {
                await AddOrUpdateSubjectMark(student.Id, "mathematics", request.Term3.Mathematics, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "science", request.Term3.Science, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "english", request.Term3.English, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "french", request.Term3.French, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "history", request.Term3.History, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "geography", request.Term3.Geography, 3, request.Year, request.Remarks);
                await AddOrUpdateSubjectMark(student.Id, "physical", request.Term3.Physical, 3, request.Year, request.Remarks);
            }

            await _context.SaveChangesAsync();

            // Return the complete updated mark report
            var updatedMarks = await _context.marks
                .Where(m => m.StudentId == student.Id && m.Year == request.Year)
                .Include(m => m.subject)
                .ToListAsync();

            return Ok(CreateMarkResponse(student, updatedMarks,request.Year));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<MarkDto>> GetStudentMarks(int studentId, [FromQuery] int year)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var marks = await _context.marks
                .Where(m => m.StudentId == student.Id && m.Year == year)
                .Include(m => m.subject)
                .ToListAsync();

            return Ok(CreateMarkResponse(student, marks, year));
        }

        [HttpGet("class/{classId}")]
        public async Task<ActionResult<List<MarkDto>>> GetClassMarks(int classId, [FromQuery] int year)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();

            if (!students.Any())
            {
                return NotFound("No students found in this class");
            }

            var result = new List<MarkDto>();

            foreach (var student in students)
            {
                var marks = await _context.marks
                    .Where(m => m.StudentId == student.Id && m.Year == year)
                    .Include(m => m.subject)
                    .ToListAsync();

                result.Add(CreateMarkResponse(student, marks,year));
            }

            return Ok(result);
        }

        [HttpDelete("remove-mark")]
        public async Task<ActionResult<MarkDto>> RemoveMark([FromBody] MarkDeleteRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var mark = await _context.marks
                .Include(m => m.student)
                .Include(m => m.subject)
                .FirstOrDefaultAsync(m =>
                    m.StudentId == student.Id &&
                    m.SubjectId == request.Subjectname &&
                    m.Semestre == request.Semester &&
                    m.Year == request.Year);

            if (mark == null)
            {
                return NotFound("Mark not found");
            }

            _context.marks.Remove(mark);
            await _context.SaveChangesAsync();

            // Return the updated marks for the student
            var updatedMarks = await _context.marks
                .Where(m => m.StudentId == student.Id && m.Year == request.Year)
                .Include(m => m.subject)
                .ToListAsync();

            return Ok(CreateMarkResponse(mark.student, updatedMarks,request.Year));
        }
    }

    // DTOs for requests and responses
    public class MarkUpdateRequest
    {
        public int StudentId { get; set; }
        public string SubjectName { get; set; }
        public double Mark { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
        public string Remarks { get; set; }
    }

    public class MarkFullUpdateRequest
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public TermMarksDto Term1 { get; set; }
        public TermMarksDto Term2 { get; set; }
        public TermMarksDto Term3 { get; set; }
        public int Year { get; set; }
        public string Remarks { get; set; }
    }

    public class MarkDeleteRequest
    {
        public int StudentId { get; set; }
        public int Subjectname { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }

    public class TermMarksDto
    {
        public double Mathematics { get; set; }
        public double Science { get; set; }
        public double English { get; set; }
        public double French { get; set; }
        public double History { get; set; }
        public double Geography { get; set; }
        public double Physical { get; set; }
    }

    public class MarkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public int Year { get; set; }
        public TermMarksDto Term1 { get; set; }
        public TermMarksDto Term2 { get; set; }
        public TermMarksDto Term3 { get; set; }
        public double FinalAverage { get; set; }
        public string Remarks { get; set; }
    }
}