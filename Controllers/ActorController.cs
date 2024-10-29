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
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ActorController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Actor
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            var movies = await _context.MovieActor
            .Include(cs => cs.Movie)
            .Where(cs => cs.ActorId == actor.Id)
            .Select(cs => cs.Movie)
            .ToListAsync();

            //////////////////////////////////////////////////////

            var ApiKey = _config["OpenAi:Key"] ?? throw new Exception("OpenAI: Key does not exist in the curent configuration");
            var ApiEndpoint = _config["OpenAi:Endpoint"] ?? throw new Exception("OpenAI: Endpoint does not exist in the current configuration");
            var AiDeployment = "gpt-35-turbo";
            ApiKeyCredential ApiCredential = new(ApiKey);

            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), ApiCredential).GetChatClient(AiDeployment);

            var ActorName = actor.Name;

            var messages = new ChatMessage[]
            {
            new SystemChatMessage("You represent the Twitter social media platform. One response should contain the username and tweet. Generate 21 responses, each seperated by a '|'"),
            new UserChatMessage($"Generate users and their tweets about the actor {ActorName}.")
            };

            var ChatCompletions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1000,
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] twitter = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToArray();

            var analyzer = new SentimentIntensityAnalyzer();
            double sentimentTotal = 0;
            var tweets_and_sentiments = new List<object[]>();



            for (int i = 0; i < twitter.Length; i++)
            {
                string review = twitter[i];
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(review);
                sentimentTotal += sentiment.Compound;

                tweets_and_sentiments.Add(new Object[] { review, sentiment.Compound });
            }

            double sentimentAverage = sentimentTotal / twitter.Length;


            var vm = new ActorDetailsViewModel(actor, movies, tweets_and_sentiments, sentimentAverage);

            //var vm = new ActorDetailsViewModel(actor, movies);
            return View(vm);
        }

        // GET: Actor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,ImdbLink,Photo")] Actor actor, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                // Convert the file to a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await Photo.CopyToAsync(memoryStream);
                    actor.Photo = memoryStream.ToArray();  // Store the image as a byte array
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbLink,Photo")] Actor actor, IFormFile Photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (Photo != null && Photo.Length > 0)
            {
                // Convert the file to a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await Photo.CopyToAsync(memoryStream);
                    actor.Photo = memoryStream.ToArray();  // Store the image as a byte array
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
