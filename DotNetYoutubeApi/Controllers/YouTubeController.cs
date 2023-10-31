using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using DotNet7YoutubeApi.Models;

namespace DotNet7YoutubeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YouTubeController : Controller
    { 
        [HttpGet]
        public async Task<IActionResult> GetChannelVideos(string? pageToken= null, int maxResults = 50) 
        {
            // Validate the maxResults parameter.
            if (maxResults > 50)
            {
                return BadRequest("The maximum number of results is 50.");
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyAw83cWkPvIni1_Kux2xYyxyotDAiwSV3k",
                ApplicationName = "MyYoutubeApp"
            }) ;

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.ChannelId = "UC8butISFwT-Wl7EV0hUK0BQ";
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
            searchRequest.MaxResults = maxResults;
            searchRequest.PageToken = pageToken;

            var searchResponse = await searchRequest.ExecuteAsync();

            var videoList = searchResponse.Items.Select(item => new VideoDetails 
            { 
                Title = item.Snippet.Title,
                Link = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
                Thumbnail = item.Snippet.Thumbnails.Medium.Url,
                PublishedAt = item.Snippet.PublishedAtDateTimeOffset
            })
            .OrderByDescending(video => video.PublishedAt)
                .ToList();

            var response = new YouTubeResponse
            {
                Videos = videoList,
                NextPageToken = searchResponse.NextPageToken,
                PrevPageToken = searchResponse.PrevPageToken
            };

            return Ok(response);
        }
    }
}
