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
        //[HttpPost("add-full-mark")]
        //public async Task<ActionResult<MarkDto>> AddFullMark(int year,MarkDto markDto)
        //{
        //    // Get or create student
        //    var student = await _context.Students.FindAsync(markDto.StudentId);
        //    if (student == null)
        //    {
        //        return NotFound("Student not found");
        //    }

        //    // Process Term1 marks
        //    await ProcessTermMarks(markDto.StudentId, 1,year, markDto.Term1);

        //    // Process Term2 marks
        //    await ProcessTermMarks(markDto.StudentId, 2,year, markDto.Term2);

        //    // Process Term3 marks
        //    await ProcessTermMarks(markDto.StudentId, 3,year, markDto.Term3);

        //    // Save remarks if needed
        //    if (!string.IsNullOrEmpty(markDto.Remarks))
        //    {
        //        // Update remarks for all marks or create a specific remarks entity
        //        // This depends on your data model
        //    }

        //    return CreatedAtAction(nameof(GetStudentMarks), new { studentId = markDto.StudentId }, markDto);
        //}

        //private async Task ProcessTermMarks(int studentId, int term,int year, TermMarksDto termMarks)
        //{
        //    // Mathematics
        //    await AddOrUpdateMark(studentId, "mathematics", termMarks.Mathematics, term,year);

        //    // Science
        //    await AddOrUpdateMark(studentId, "science", termMarks.Science, term,year);

        //    // English
        //    await AddOrUpdateMark(studentId, "english", termMarks.English, term,year);

        //    // French
        //    await AddOrUpdateMark(studentId, "french", termMarks.French, term,year);

        //    // History
        //    await AddOrUpdateMark(studentId, "history", termMarks.History, term, year);

        //    // Geography
        //    await AddOrUpdateMark(studentId, "geography", termMarks.Geography, term,year);

        //    // Physical
        //    await AddOrUpdateMark(studentId, "physical", termMarks.Physical, term,year);
        //}

        //private async Task AddOrUpdateMark(int studentId, string subjectName, double markValue, int term, int year)
        //{
        //    if (markValue <= 0) return; // Skip if mark is 0 or negative

        //    var subject = await _context.subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == subjectName.ToLower());
        //    if (subject == null)
        //    {
        //        // Create subject if it doesn't exist (optional)
        //        subject = new Subject { Name = subjectName };
        //        _context.subjects.Add(subject);
        //        await _context.SaveChangesAsync();
        //    }

        //    var existingMark = await _context.marks
        //        .FirstOrDefaultAsync(m =>
        //            m.StudentId == studentId &&
        //            m.SubjectId == subject.id &&
        //            m.Semestre == term && m.Year==year);

        //    if (existingMark != null)
        //    {
        //        // Update existing mark
        //        existingMark.mark = Convert.ToInt32(markValue);
        //    }
        //    else
        //    {
        //        // Add new mark
        //        var newMark = new Marks
        //        {
        //            StudentId = studentId,
        //            SubjectId = subject.id,
        //            mark = Convert.ToInt32(markValue),
        //            Semestre = term,
        //            Year = year,
        //             Remarque = remarque
        //        };
        //        _context.marks.Add(newMark);
        //    }

        //    await _context.SaveChangesAsync();
        //}
        [HttpPost("add-full-mark")]
        public async Task<ActionResult<MarkDto>> AddFullMark(int year, MarkDto markDto)
        {
            // Validate student exists
            var student = await _context.Students.FindAsync(markDto.StudentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            // Process each term's marks
            await ProcessTermMarks(markDto.StudentId, 1, year, markDto.Term1, markDto.Remarks);
            await ProcessTermMarks(markDto.StudentId, 2, year, markDto.Term2, markDto.Remarks);
            await ProcessTermMarks(markDto.StudentId, 3, year, markDto.Term3, markDto.Remarks);

            // Return the complete mark report with calculated averages
            return await GetCompleteStudentMarkReport(markDto.StudentId, year);
        }
        private async Task AddOrUpdateSubjectMark(int studentId, string subjectName, double markValue, int term, int year, string remarks)
        {
            if (markValue < 0) return; // Skip if mark is negative

            // Find or create subject
            var subject = await _context.subjects
                .FirstOrDefaultAsync(s => s.Name.ToLower() == subjectName.ToLower());

            if (subject == null)
            {
                subject = new Subject { Name = subjectName };
                _context.subjects.Add(subject);
                await _context.SaveChangesAsync();
            }

            // Check if mark already exists
            var existingMark = await _context.marks
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.SubjectId == subject.id &&
                    m.Semestre == term &&
                    m.Year == year);

            if (existingMark != null)
            {
                // Update existing mark
                existingMark.mark = Convert.ToInt32(markValue);
                existingMark.Remarque = remarks;
            }
            else
            {
                // Create new mark
                var newMark = new Marks
                {
                    StudentId = studentId,
                    SubjectId = subject.id,
                    mark = Convert.ToInt32(markValue),
                    Semestre = term,
                    Year = year,
                    Remarque = remarks
                };
                _context.marks.Add(newMark);
            }

            await _context.SaveChangesAsync();
        }
        private async Task ProcessTermMarks(int studentId, int term, int year, TermMarksDto termMarks, string remarks)
        {
            await AddOrUpdateSubjectMark(studentId, "mathematics", termMarks.Mathematics, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "science", termMarks.Science, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "english", termMarks.English, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "french", termMarks.French, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "history", termMarks.History, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "geography", termMarks.Geography, term, year, remarks);
            await AddOrUpdateSubjectMark(studentId, "physical", termMarks.Physical, term, year, remarks);
        }

        private async Task<MarkDto> GetCompleteStudentMarkReport(int studentId, int year)
        {
            var student = await _context.Students
                .Where(s => s.Id == studentId)
                .Include(s => s.marks)
                    .ThenInclude(m => m.subject)
                .FirstOrDefaultAsync();

            if (student == null) return null;

            var yearMarks = student.marks.Where(m => m.Year == year).ToList();

            // Get marks by term
            var term1Marks = yearMarks.Where(m => m.Semestre == 1).ToList();
            var term2Marks = yearMarks.Where(m => m.Semestre == 2).ToList();
            var term3Marks = yearMarks.Where(m => m.Semestre == 3).ToList();

            // Calculate term averages (simple average of all marks in the term)
            var term1Average = term1Marks.Any() ? term1Marks.Average(m => m.mark) : 0;
            var term2Average = term2Marks.Any() ? term2Marks.Average(m => m.mark) : 0;
            var term3Average = term3Marks.Any() ? term3Marks.Average(m => m.mark) : 0;

            // Calculate final average (average of term averages)
            var finalAverage = (term1Average + term2Average + term3Average) / 3;

            // Create subject dictionaries for response
            var term1Dict = term1Marks.ToDictionary(m => m.subject.Name.ToLower(), m => (double)m.mark);
            var term2Dict = term2Marks.ToDictionary(m => m.subject.Name.ToLower(), m => (double)m.mark);
            var term3Dict = term3Marks.ToDictionary(m => m.subject.Name.ToLower(), m => (double)m.mark);

            return new MarkDto
            {
                Id = student.Id,
                Name = student.Name,
                StudentId = student.Id,
                Term1 = new TermMarksDto
                {
                    Mathematics = term1Dict.GetValueOrDefault("mathematics", 0),
                    Science = term1Dict.GetValueOrDefault("science", 0),
                    English = term1Dict.GetValueOrDefault("english", 0),
                    French = term1Dict.GetValueOrDefault("french", 0),
                    History = term1Dict.GetValueOrDefault("history", 0),
                    Geography = term1Dict.GetValueOrDefault("geography", 0),
                    Physical = term1Dict.GetValueOrDefault("physical", 0)
                },
                Term2 = new TermMarksDto
                {
                    // Same pattern for term 2
                },
                Term3 = new TermMarksDto
                {
                    // Same pattern for term 3
                },
                FinalAverage = finalAverage,
                Remarks = yearMarks.FirstOrDefault()?.Remarque ?? "No remarks"
            };
        }

        [HttpGet("semester-average")]
        public async Task<ActionResult<double>> GetSemesterAverage(
    [FromQuery] int studentId,
    [FromQuery] int semester,
    [FromQuery] int year)
        {
            // Get all marks for the specified criteria
            var marks = await _context.marks
                .Where(m => m.StudentId == studentId &&
                           m.Semestre == semester &&
                           m.Year == year)
                .Select(m => m.mark)
                .ToListAsync();

            if (!marks.Any())
            {
                return NotFound("No marks found");
            }

            // Calculate and return the average
            return Ok(marks.Average());
        }


    //    [HttpGet("top-students-by-semester")]
    //    public async Task<ActionResult<List<StudentAverageDto>>> GetTopStudentsBySemester(
    //[FromQuery] int levelId,
    //[FromQuery] int semester,
    //[FromQuery] int year,
    //[FromQuery] int count = 5)
    //    {
    //        // 1. Get students in the specified class/level
    //        var students = await _context.Students
    //            .Where(s => s.ClassId == levelId)
    //            .Select(s => new { s.Id, s.Name })
    //            .ToListAsync();

    //        if (!students.Any())
    //            return NotFound("No students found in this class");

    //        // 2. Get all marks for these students in the specified semester/year
    //        var marks = await _context.marks
    //            .Where(m => m.Semestre == semester && m.Year == year &&
    //                       students.Select(s => s.Id).Contains(m.StudentId))
    //            .ToListAsync();

    //        // 3. Calculate semester average for each student
    //        var results = students
    //            .Select(s => new StudentAverageDto
    //            {
    //                StudentId = s.Id,
    //                StudentName = s.Name,
    //                Average = marks
    //                    .Where(m => m.StudentId == s.Id)
    //                    .Select(m => (double)m.mark)
    //                    .DefaultIfEmpty(0)
    //                    .Average()
    //            })
    //            .Where(s => s.Average > 0) // Exclude students with 0 average
    //            .OrderByDescending(s => s.Average) // Order by semester average (highest first)
    //            .Take(count)
    //            .ToList();

    //        if (!results.Any())
    //            return NotFound("No marks found for this semester");

    //        return Ok(results);
    //    }

        //[HttpPost]
        //public async Task<ActionResult<Mark_Dto>> CreateMark(Mark_Dto markDto)
        //{
        //    var mark = markDto.ToMark();
        //    _context.marks.Add(mark);
        //    await _context.SaveChangesAsync();

        //    markDto.Id = mark.Id;
        //    return CreatedAtAction(nameof(GetMarkById), new { id = mark.Id }, markDto);
        //}

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

