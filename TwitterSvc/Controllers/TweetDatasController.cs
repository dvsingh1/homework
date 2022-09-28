using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TwitterSvc.Models;

namespace TwitterSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetDatasController : ControllerBase
    {
        private readonly TwitterContext _context;
        private readonly IDataStore dataStore;
        private readonly IStartWorker startWorker;

        public TweetDatasController(TwitterContext context, IStartWorker startWorker, IDataStore dataStore)
        {
            _context = context;
            this.startWorker = startWorker;
            this.dataStore = dataStore;
            startWorker.setManualResetEvent();
        }

        public async ValueTask<int> SaveDataAsync(int count)
        {
            return count;
        }
        // GET: api/TweetDatas
        [HttpGet]
        public async Task<ActionResult<HashtagData>> GetTweetData()
        {
            HashtagData hashtag1 = new HashtagData();

            IDictionary<string, int> dict;
            hashtag1.hashtagCount = dataStore.GetPopularHashtagsData(10);
            hashtag1.tweetCount = dataStore.GetTweetCount();
            string result = JsonConvert.SerializeObject(hashtag1);
            OkObjectResult ok = Ok(result);
            return ok;
        }

        // GET: api/TweetDatas/5
        [HttpGet("{count}")]
        public async Task<ActionResult<IEnumerable<TweetData>>> GetTweetData(int count)
        {

            int tweetCount = dataStore.GetTweetCount();
            IList<TweetData> list = new List<TweetData>();
            int index = tweetCount - count;
            int end = tweetCount;
            if (index < 0)
            {
                index = 0; 
            }

            for (; index < end; index++)
            { 
                TweetData data = dataStore.GetTweetEntry(index);
                list.Add(data);
            }
            string result = JsonConvert.SerializeObject(list);
            OkObjectResult ok = Ok(result);
            return ok;

        }

        // PUT: api/TweetDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTweetData(long id, TweetData tweetData)
        {
            if (id != tweetData.id)
            {
                return BadRequest();
            }

            _context.Entry(tweetData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TweetDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TweetDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TweetData>> PostTweetData(TweetData tweetData)
        {
            _context.TweetData.Add(tweetData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTweetData", new { id = tweetData.id }, tweetData);
        }

        // DELETE: api/TweetDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTweetData(long id)
        {
            var tweetData = await _context.TweetData.FindAsync(id);
            if (tweetData == null)
            {
                return NotFound();
            }

            _context.TweetData.Remove(tweetData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TweetDataExists(long id)
        {
            return _context.TweetData.Any(e => e.id == id);
        }
    }
}
