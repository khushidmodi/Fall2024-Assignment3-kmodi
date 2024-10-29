using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_kmodi.Data;
using Fall2024_Assignment3_kmodi.Models;
using OpenAI.Chat;
using System.Text.Json.Nodes;
using Azure.AI.OpenAI;
using System.ClientModel;
using VaderSharp2;

namespace Fall2024_Assignment3_kmodi.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public MovieController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var actors = await _context.MovieActor
            .Include(cs => cs.Actor)
            .Where(cs => cs.MovieId == movie.Id)
            .Select(cs => cs.Actor)
            .ToListAsync();

            /////////////////////

            var ApiKey = _config["OpenAi:Key"] ?? throw new Exception("OpenAI: Key does not exist in the curent configuration");
            var ApiEndpoint = _config["OpenAi:Endpoint"] ?? throw new Exception("OpenAI: Endpoint does not exist in the current configuration");
            var AiDeployment = "gpt-35-turbo";
            ApiKeyCredential ApiCredential = new(ApiKey);

            var MovieName = movie.Name;
            var YearOfRelease = movie.YearOfRelease;

            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), ApiCredential).GetChatClient(AiDeployment);
            var analyzer = new SentimentIntensityAnalyzer();
            double sentimentTotal = 0;
            var reviews_and_sentiments = new List<Object[]>();

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy", "hates movies", "is easily impressed", "dislikes romance", "dislikes horror", "dislikes fantasy" };



            var messages = new ChatMessage[]
            {
            //new SystemChatMessage($"You represent a group of {personas.Length} film critics who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group with each response separated by a '|', but don't indicate which member you are. Each response should contain a rating out of 10 and the member's comments."),
            //new UserChatMessage($"How would you rate the movie {MovieName} released in {YearOfRelease} out of 10 in 150 words or less?")
            new SystemChatMessage($"You represent a group of {personas.Length} film critics with the following personalities: {string.Join(", ", personas)}. When asked a question, respond as a member of the group. Each member will respond once, so there will be 10 responses. Each response should be separated by a '|', formatted as follows: 'Comments. #/10'. Only provide responses in this exact format, with comments followed by a rating out of 10."),
            new UserChatMessage($"Rate the movie {MovieName} (released in {YearOfRelease})."),
            };


            var ChatCompletions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1000,
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] reviews = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToArray();

            for (int i = 0; i < reviews.Length; i++)
            {
                string review = reviews[i];
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(review);
                sentimentTotal += sentiment.Compound;

                reviews_and_sentiments.Add(new Object[] { review, sentiment.Compound });
            }

            double sentimentAverage = sentimentTotal / reviews.Length;

            var vm = new MovieDetailsViewModel(movie, actors, reviews_and_sentiments, sentimentAverage);

            //return View(movie);
            return View(vm);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImdbLink,Genre,YearOfRelease,Photo")] Movie movie, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                // Convert the file to a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await Photo.CopyToAsync(memoryStream);
                    movie.Photo = memoryStream.ToArray();  // Store the image as a byte array
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImdbLink,Genre,YearOfRelease,Photo")] Movie movie, IFormFile Photo)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (Photo != null && Photo.Length > 0)
            {
                // Convert the file to a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await Photo.CopyToAsync(memoryStream);
                    movie.Photo = memoryStream.ToArray();  // Store the image as a byte array
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
