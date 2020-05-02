using Core.Interfaces;
using Core.Models.ViewModels;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class YoutubeService
    {
        private readonly IRepositoryYoutube repository;
        private readonly YouTubeService youTubeDataApiService;
        
        public YoutubeService(
            IRepositoryYoutube repository,
            YouTubeService youTubeDataApiService
            )
        {
            this.repository = repository;
            this.youTubeDataApiService = youTubeDataApiService;
        }

        public async Task VideoAdd(Core.Models.Video video)
        {
            await repository.VideoAdd(video);
        }

        public async Task VideoUpdate(Core.Models.Video video)
        {
            await repository.VideoUpdate(video);
        }

        public async Task<IEnumerable<Core.Models.Video>> VideoList()
        {
            return await repository.VideoList();
        }

        public async Task<Core.Models.Video> VideoById(string id)
        {
            return await repository.VideoById(id);
        }

        public async Task VideoDelAll()
        {
            await repository.VideoDelAll();
        }

        public async Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel)
        {
            await repository.AddVideoToPlaylist(videoPlaylistViewModel);
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
