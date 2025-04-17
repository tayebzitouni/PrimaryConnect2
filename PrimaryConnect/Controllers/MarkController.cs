using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkController : Controller
    {
        public MarkController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        private AppDbContext _context;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mark_Dto>>> GetAllMarks()
        {
            return await _context.marks
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .ToListAsync();
        }

        // GET: api/Marks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Mark_Dto>> GetMarkById(int id)
        {
            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            return new Mark_Dto
            {
                Id = mark.Id,
                subjectid = mark.SubjectId,
                Mark = mark.mark,
                Remarque = mark.Remarque,
                Semestre = mark.Semestre,
                Year = mark.Year,
                StudentId = mark.StudentId
            };
        }

        // GET: api/Marks/get-student-marks?studentId=1&semester=1&year=2024
        [HttpGet("get-student-marks")]
        public async Task<ActionResult<IEnumerable<Mark_Dto>>> GetStudentMarks(int studentId, int semester, int year)
        {
            var marks = await _context.marks
                .Where(m => m.StudentId == studentId && m.Semestre == semester && m.Year == year)
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .ToListAsync();

            if (!marks.Any())
                return NotFound("No marks found for the specified parameters.");

            return Ok(marks);
        }

        // POST: api/Marks
        [HttpPost]
        public async Task<ActionResult<Mark_Dto>> CreateMark(Mark_Dto markDto)
        {
            var mark = markDto.ToMark();
            _context.marks.Add(mark);
            await _context.SaveChangesAsync();

            markDto.Id = mark.Id;
            return CreatedAtAction(nameof(GetMarkById), new { id = mark.Id }, markDto);
        }

        // PUT: api/Marks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMark(int id, Mark_Dto markDto)
        {
            if (id != markDto.Id)
                return BadRequest();

            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            mark.SubjectId = markDto.subjectid;
            mark.mark = markDto.Mark;
            mark.Remarque = markDto.Remarque;
            mark.Semestre = markDto.Semestre;
            mark.Year = markDto.Year;
            mark.StudentId = markDto.StudentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Marks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            _context.marks.Remove(mark);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("student-mark")]
        public async Task<ActionResult<Mark_Dto>> GetStudentMarkBySubject(int studentId, int subjectId, int semestre)
        {
            var mark = await _context.marks
                .Where(m => m.StudentId == studentId && m.SubjectId == subjectId && m.Semestre == semestre)
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .FirstOrDefaultAsync();

            if (mark == null)
                return NotFound("Mark not found for the given student and subject.");

            return Ok(mark);
        }

        [HttpGet("class-marks/{classId}")]
        public async Task<ActionResult<IEnumerable<Mark_Dto>>> GetMarksByClass(int classId)
        {
            
            var studentIds = await _context.Students
                .Where(s => s.ClassId == classId)
                .Select(s => s.Id)
                .ToListAsync();

            if (!studentIds.Any())
                return NotFound("No students found in the given class.");

           
            var marks = await _context.marks
                .Where(m => studentIds.Contains(m.StudentId))
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .ToListAsync();

            if (!marks.Any())
                return NotFound("No marks found for the students in this class.");

            return Ok(marks);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetMarksByStudent(int studentId)
        {
            var student = await _context.Students
                .Where(s => s.Id == studentId)
                .Include(s => s.marks)
                .ThenInclude(m => m.subject)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            // Group marks by semester and map to subject names (lowercase for consistency)
            var term1Marks = student.marks.Where(m => m.Semestre == 1)
                .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);
            var term2Marks = student.marks.Where(m => m.Semestre == 2)
                .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);
            var term3Marks = student.marks.Where(m => m.Semestre == 3)
                .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

          
            var sem1Avg = student.marks.Where(m => m.Semestre == 1).Select(m => m.mark).DefaultIfEmpty(0).Average();
            var sem2Avg = student.marks.Where(m => m.Semestre == 2).Select(m => m.mark).DefaultIfEmpty(0).Average();
            var sem3Avg = student.marks.Where(m => m.Semestre == 3).Select(m => m.mark).DefaultIfEmpty(0).Average();
            var finalAverage = (sem1Avg + sem2Avg + sem3Avg) / 3;

           
            var remarks = student.marks.FirstOrDefault()?.Remarque ?? "No remarks";

            
            var result = new MarkDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentId = student.Id,
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
                Remarks = remarks
            };

            return Ok(result);
        }


        [HttpGet("class/{classId}/students")]
        public async Task<IActionResult> GetStudentsByClass(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .Include(s => s.marks)
                    .ThenInclude(m => m.subject)
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found for this class.");
            }

            var studentDtos = students.Select(student =>
            {
                var term1Marks = student.marks
                    .Where(m => m.Semestre == 1)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var term2Marks = student.marks
                    .Where(m => m.Semestre == 2)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var term3Marks = student.marks
                    .Where(m => m.Semestre == 3)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var allMarks = student.marks.Select(m => m.mark);
                var sem1Avg = student.marks.Where(m => m.Semestre == 1).Select(m => m.mark).DefaultIfEmpty(0).Average();
                var sem2Avg = student.marks.Where(m => m.Semestre == 2).Select(m => m.mark).DefaultIfEmpty(0).Average();
                var sem3Avg = student.marks.Where(m => m.Semestre == 3).Select(m => m.mark).DefaultIfEmpty(0).Average();
                var finalAverage = (sem1Avg + sem2Avg + sem3Avg) / 3;

                var remarks = student.marks.FirstOrDefault()?.Remarque ?? "No remarks";

                return new MarkDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    StudentId = student.Id,
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
                    Remarks = remarks
                };
            }).ToList();

            return Ok(studentDtos);
        }

        [HttpGet("students/marks")]
        public async Task<IActionResult> GetAllStudentsWithMarks()
        {
            var students = await _context.Students
                .Include(s => s.marks)
                    .ThenInclude(m => m.subject)
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("No students found.");
            }

            var studentDtos = students.Select(student =>
            {
                var term1Marks = student.marks
                    .Where(m => m.Semestre == 1)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var term2Marks = student.marks
                    .Where(m => m.Semestre == 2)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var term3Marks = student.marks
                    .Where(m => m.Semestre == 3)
                    .ToDictionary(m => m.subject.Name.ToLower(), m => m.mark);

                var allMarks = student.marks.Select(m => m.mark);
                var finalAverage = allMarks.Any() ? allMarks.Average() : 0;

                var remarks = student.marks.FirstOrDefault()?.Remarque ?? "No remarks";

                return new MarkDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    StudentId = student.Id,
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
                    Remarks = remarks
                };
            }).ToList();

            return Ok(studentDtos);
        }


        [HttpGet("class/{classId}/subjects/averages")]
        public async Task<IActionResult> GetSubjectAveragesByClass(int classId)
        {
            var marks = await _context.marks
                .Include(m => m.subject)
                .Include(m => m.student)
                .Where(m => m.student.ClassId == classId)
                .ToListAsync();

            var subjectGroups = marks
                .GroupBy(m => m.subject.Name.ToLower())
                .Select(g =>
                {
                    var semester1 = g.Where(m => m.Semestre == 1).Select(m => m.mark);
                    var semester2 = g.Where(m => m.Semestre == 2).Select(m => m.mark);
                    var semester3 = g.Where(m => m.Semestre == 3).Select(m => m.mark);

                    double avg1 = semester1.Any() ? semester1.Average() : 0;
                    double avg2 = semester2.Any() ? semester2.Average() : 0;
                    double avg3 = semester3.Any() ? semester3.Average() : 0;
                    double overall = (avg1 + avg2 + avg3) / 3;

                    return new
                    {
                        Subject = g.Key,
                        Semester1 = avg1,
                        Semester2 = avg2,
                        Semester3 = avg3,
                        OverallAverage = overall
                    };
                })
                .ToDictionary(
                    x => x.Subject,
                    x => new
                    {
                        semester1 = x.Semester1,
                        semester2 = x.Semester2,
                        semester3 = x.Semester3,
                        overallAverage = x.OverallAverage
                    });

            return Ok(subjectGroups);
        }


    }
}

