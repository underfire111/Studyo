﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Studyo.Data;
using Studyo.Models;

namespace Studyo.Controllers
{
    public class DisciplinasController : Controller
    {

        private readonly StudyoDbContext _context;
        private readonly UserManager<IdentityUser> _userManger;

        public DisciplinasController(StudyoDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManger = userManager;
        }

        [HttpGet("get-subjects")]
        public async Task<List<Subject>> GetSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<IActionResult> GetSubjectsAsync()
        {
            var subjects = await _context.Subjects.ToListAsync();

            if (subjects == null || !subjects.Any()) { return NotFound(); }

            return View(subjects);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AlgoritmoChapterId()
        {
            IdentityUser? user = await _userManger.GetUserAsync(User);
            List<UserSubjects> currentUserSubjects = _context.UsersSubjects.Where(d => d.UserId == user.Id).ToList();

            int idChapter;

            if (currentUserSubjects.Count == 0) { idChapter = -1; }
            else
            {
                List<Chapter> listChaptersOfSubject = new List<Chapter>();

                foreach (KeyValuePair<Chapter, bool> t in currentUserSubjects.OrderBy(s => s.calculateCompletion()).First().CompletedChapters)
                {
                    if (t.Value == false)
                    {
                        listChaptersOfSubject.Add(t.Key);
                    }
                }

                idChapter = listChaptersOfSubject.OrderBy(c => c.Id).First().Id;
            }

            if (idChapter == -1) { return NotFound(); }

            Chapter chap = _context.Chapters.Where(c => c.Id == idChapter).First();

            return View(chap);
        }
    }
}