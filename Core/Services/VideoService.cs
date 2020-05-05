using Core.Interfaces;
using Core.ViewModels;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class VideoService
    {
        private readonly IRepositoryVideo repository;
        private readonly YouTubeService youTubeDataApiService;
        
        public VideoService(
            IRepositoryVideo repository,
            YouTubeService youTubeDataApiService
            )
        {
            this.repository = repository;
            this.youTubeDataApiService = youTubeDataApiService;
        }

        public async Task VideoAdd(Models.Video video)
        {
            await repository.VideoAdd(video);
        }

        public async Task VideoUpdate(Models.Video video)
        {
            await repository.VideoUpdate(video);
        }

        public async Task SaveVideo(Models.Video video)
        {
            if (await VideoById(video.Id) == null)
            {
                await VideoAdd(video);
            }
            else
            {
                await VideoUpdate(video);
            }
        }

        public async Task<IEnumerable<Models.Video>> VideoList()
        {
            return await repository.VideoList();
        }

        public async Task<Models.Video> VideoById(string id)
        {
            return await repository.VideoById(id);
        }

        public async Task<Models.Video> VideoDetailById(string id)
        {
            return await repository.VideoDetailById(id);
        }

        public async Task VideoDelAll()
        {
            await repository.VideoDelAll();
        }

        public async Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel)
        {
            await repository.AddVideoToPlaylist(videoPlaylistViewModel);
        }

        public async Task<IEnumerable<Models.Video>> Search(string searchText)
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

            List<Models.Video> videos = new List<Models.Video>();

            foreach (SearchResult searchResult in searchResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        var video = new Models.Video()
                            {
                                Id = searchResult.Id.VideoId,
                                Title = searchResult.Snippet.Title,
                                Description = searchResult.Snippet.Description,
                                Url = searchResult.Snippet.Thumbnails.Medium.Url
                            };

                        videos.Add(video);
                        await SaveVideo(video);
                        break;
                }
            }
            return await LoadPlaylistsInVideo(videos);
        }

        private async Task<IEnumerable<Models.Video>> LoadPlaylistsInVideo(IEnumerable<Models.Video> videos)
        {
            var resultVideos = new List<Models.Video>();
            foreach (var video in videos)
            {
                resultVideos.Add(await repository.VideoDetailById(video.Id));
            }
            return resultVideos;
        }
    }
}
