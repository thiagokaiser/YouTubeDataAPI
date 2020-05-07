using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class VideoRepository : IRepositoryVideo
    {
        private readonly string connectionString;

        public VideoRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task VideoUpdate(Video video)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync(@"UPDATE Video SET
                                            title = @Title,
                                            description = @Description,
                                            url = @Url
                                            WHERE id = @Id", video);
            }
        }

        public async Task VideoAdd(Video video)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync("INSERT INTO Video(id, title, description, url) VALUES(@Id, @Title, @Description, @Url)", video);
            }
        }

        public async Task<Video> VideoById(string id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {                
                var video = await conexao.QueryFirstOrDefaultAsync<Video>("SELECT * FROM Video WHERE id = @Id", new { Id = id });
                return video;                
            }
        }

        public async Task<Video> VideoDetailById(string id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var query = @"
                    SELECT * FROM video 
                    LEFT JOIN videoplaylist ON videoplaylist.videoid = video.id 
                    LEFT JOIN playlist ON playlist.id = videoplaylist.playlistid
                    WHERE video.id = @Id;
                    ";

                var playlists = await conexao.QueryAsync<Video, Playlist, Video>(query,
                    (video, playlist) =>
                    {
                        video.Playlists = video.Playlists ?? new List<Playlist>();
                        video.Playlists.Add(playlist);
                        return video;
                    }, splitOn: "playlistid", param: new { Id = id });

                var grouped = playlists.GroupBy(o => o.Id)
                    .Select(group =>
                    {
                        var combinedVideo = group.First();
                        combinedVideo.Playlists = group.Select(video => video.Playlists.Single()).ToList();
                        return combinedVideo;
                    });

                return grouped.First();
            }
        }

        public async Task<IEnumerable<Video>> VideoList()
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var query = @"
                    SELECT * FROM video 
                    LEFT JOIN videoplaylist ON videoplaylist.videoid = video.id 
                    LEFT JOIN playlist ON playlist.id = videoplaylist.playlistid;
                    ";

                var playlists = await conexao.QueryAsync<Video, Playlist, Video>(query,
                    (video, playlist) =>
                    {
                        video.Playlists = video.Playlists ?? new List<Playlist>();
                        video.Playlists.Add(playlist);
                        return video;
                    }, splitOn: "playlistid");

                var grouped = playlists.GroupBy(o => o.Id)
                    .Select(group =>
                    {
                        var combinedVideo = group.First();
                        combinedVideo.Playlists = group.Select(video => video.Playlists.Single()).ToList();
                        return combinedVideo;
                    });

                return grouped;
            }
        }

        public async Task VideoDelAll()
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync("DELETE FROM Video");
                await conexao.ExecuteAsync("DELETE FROM VideoPlaylist");
            }
        }

        public async Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylist)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var result = await conexao.QueryFirstOrDefaultAsync<VideoPlaylist>(@"SELECT * FROM videoplaylist 
                                                                                     WHERE videoId = @VideoId AND playlistid = @PlaylistId", videoPlaylist);
                if (result != null)
                {
                    throw new Exception("Relacionamento já existe");
                }
                await conexao.ExecuteAsync("INSERT INTO VideoPlaylist(videoid, playlistid) VALUES(@VideoId, @PlaylistId)", videoPlaylist);
            }
        }
    }
}
