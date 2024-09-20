using HtmlAgilityPack; // Ensure to add this NuGet package for HTML parsing
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSearchController : ControllerBase
    {
        private const string GoogleSearchApiUrl = "https://www.googleapis.com/customsearch/v1";
        private const string GoogleApiKey = "AIzaSyCOyMEjku69QHX8HY3TSM07LTBi3LD7zL4";
        private const string NSTSearchEngineId = "31259719c650b42a4";
        private const string CNASearchEngineId = "f41669e09ae6d4aa8";

        private readonly RestClient _client;

        public GoogleSearchController()
        {
            _client = new RestClient(GoogleSearchApiUrl);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Article>> SearchNews(string q, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            var request = new RestRequest();

            // Set query parameters for news search
            request.AddParameter("key", GoogleApiKey);
            request.AddParameter("q", q);
            request.AddParameter("tbm", "nws"); // Restrict to news articles

            // Add date restriction parameters if provided
            if (startDate.HasValue && endDate.HasValue)
            {
                string formattedStartDate = startDate.Value.ToString("yyyy-MM-dd");
                string formattedEndDate = endDate.Value.ToString("yyyy-MM-dd");

                request.AddParameter("dateRestrict", $"cdr:1,{formattedStartDate}:{formattedEndDate}");
            }

            // Execute request for NST
            request.AddParameter("cx", NSTSearchEngineId);
            var nstResponse = _client.Execute(request);

            // Execute request for CNA
            request.AddParameter("cx", CNASearchEngineId);
            var cnaResponse = _client.Execute(request);

            var allArticles = new List<Article>();

            // Process NST response
            if (nstResponse.StatusCode == HttpStatusCode.OK)
            {
                var nstResults = JsonConvert.DeserializeObject<GoogleSearchResponse>(nstResponse.Content);
                allArticles.AddRange(ProcessSearchResults(nstResults));
            }
            else
            {
                return StatusCode((int)nstResponse.StatusCode, "NST API request failed.");
            }

            // Process CNA response
            if (cnaResponse.StatusCode == HttpStatusCode.OK)
            {
                var cnaResults = JsonConvert.DeserializeObject<GoogleSearchResponse>(cnaResponse.Content);
                allArticles.AddRange(ProcessSearchResults(cnaResults));
            }
            else
            {
                return StatusCode((int)cnaResponse.StatusCode, "CNA API request failed.");
            }

            // Scrape data from article URLs
            foreach (var article in allArticles)
            {
                ScrapeDataFromUrl(article);
            }

            return Ok(allArticles); // Return list of articles with scraped data
        }

        private void ScrapeDataFromUrl(Article article)
        {
            try
            {
                if (!Uri.TryCreate(article.Link, UriKind.Absolute, out Uri? uri))
                {
                    Console.WriteLine($"Invalid URL: {article.Link}");
                    return;
                }

                // Optionally, check for specific domain or other validations

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(uri).Result;
                    response.EnsureSuccessStatusCode(); // Ensure HTTP success status code

                    var html = response.Content.ReadAsStringAsync().Result;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    // Example: Extract text content from a specific element
                    var mainContentNode = doc.DocumentNode.SelectSingleNode("//div[@class='main-content']");
                    if (mainContentNode != null)
                    {
                        article.Text = mainContentNode.InnerText.Trim();
                    }
                    else
                    {
                        // Print out HTML for debugging
                        Console.WriteLine(doc.DocumentNode.InnerHtml);
                        Console.WriteLine($"Could not find main content node for URL: {article.Link}");
                    }

                    // Example: Extract additional data as needed

                    // Note: You may need to adjust the XPath or CSS selectors based on the structure of the target site.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping data from URL {article.Link}: {ex.Message}");
            }
        }
        private List<Article> ProcessSearchResults(GoogleSearchResponse searchResponse)
        {
            var articles = new List<Article>();

            foreach (var item in searchResponse.Items)
            {
                var fullTitle = item.Title; // Assuming item.Title already contains full title

                var article = new Article
                {
                    Title = fullTitle,
                    Link = item.Link,
                    PublishedDate = ExtractPublishedDate(item.Snippet),
                    Text = ExtractSnippetWithoutDate(item.Snippet),
                    ImageUrl = item.Image?.ThumbnailLink,
                };

                articles.Add(article);
            }

            return articles;
        }

        private string ExtractSnippetWithoutDate(string snippet)
        {
            string pattern = @"\b(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\s+\d{1,2},\s+\d{4}\b";
            return Regex.Replace(snippet, pattern, "");
        }

        private string ExtractPublishedDate(string snippet)
        {
            string pattern = @"\b(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\s+\d{1,2},\s+\d{4}\b";

            var regex = new Regex(pattern);
            var match = regex.Match(snippet);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class GoogleSearchResponse
    {
        public List<GoogleSearchItem>? Items { get; set; }
    }

    public class GoogleSearchItem
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
        public string? Snippet { get; set; }
        public GoogleSearchImage? Image { get; set; }
    }

    public class GoogleSearchImage
    {
        public string? ThumbnailLink { get; set; }
    }

    public class Article
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
        public string? Snippet { get; set; }
        public string? PublishedDate { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
    }
}
