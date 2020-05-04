using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Video>> VideoList()
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var videos = await conexao.QueryAsync<Core.Models.Video>("SELECT * FROM Video");
                return videos;
            }
        }

        public async Task VideoDelAll()
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync("DELETE FROM Video");
            }
        }

        public async Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync("INSERT INTO VideoPlaylist(videoid, playlistid) VALUES(@VideoId, @PlaylistId)", videoPlaylistViewModel);
            }
        }
    }
}
