using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Api.Apis
{
    public class YoutubeApi
    {
        private readonly YouTubeService youTubeDataApiService;
        public YoutubeApi(Google.Apis.YouTube.v3.YouTubeService youTubeDataApiService)
        {            
            this.youTubeDataApiService = youTubeDataApiService;            
        }

        public IEnumerable<Core.Models.Video> Search(string searchText)
        {
            SearchResource.ListRequest listRequest = youTubeDataApiService.Search.List("snippet");

            listRequest.Q = searchText;
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            listRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;
            listRequest.RegionCode = "BR";
            listRequest.RelevanceLanguage = "PT-BR";
            listRequest.Type = "video";
            listRequest.MaxResults = 10;

            SearchListResponse searchResponse = listRequest.Execute();

            List<Core.Models.Video> videos = new List<Core.Models.Video>();

            foreach (SearchResult searchResult in searchResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(new Core.Models.Video()
                        {
                            Id = searchResult.Id.VideoId,
                            Title = searchResult.Snippet.Title,
                            Description = searchResult.Snippet.Description,
                            Url = searchResult.Snippet.Thumbnails.Medium.Url
                        });
                        break;
                }
            }

            return videos;
        }
    }
}
