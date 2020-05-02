using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Dapper;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class PlaylistRepository : IRepositoryPlaylist
    {
        private readonly string connectionString;

        public PlaylistRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Playlist> PlaylistById(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var playlist = await conexao.QueryFirstOrDefaultAsync<Playlist>("SELECT * FROM Playlist WHERE id = @Id", new { Id = id });
                return playlist;
            }
        }

        public async Task<IEnumerable<Video>> PlaylistVideosById(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var videos = await conexao.QueryAsync<Video>("SELECT * FROM Video WHERE playlistid = @Id", new { Id = id });
                return videos;
            }
        }

        public async Task<IEnumerable<Playlist>> PlaylistList()
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var playlists = await conexao.QueryAsync<Playlist>("SELECT * FROM Playlist");
                return playlists;
            }
        }

        public async Task VideoDel(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var playlist = await conexao.ExecuteAsync(@"DELETE FROM Playlist                                                          
                                                             WHERE id = @Id", new { Id = id });
            }

        }

        public async Task PlaylistDel(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var playlist = await conexao.ExecuteAsync(@"DELETE FROM Playlist                                                          
                                                             WHERE id = @Id", new { Id = id });
            }
        }

        public async Task PlaylistUpdate(Playlist playlist)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync(@"UPDATE Playlist SET
                                            title = @Title,
                                            description = @Description
                                            WHERE id = @Id", playlist);
            }
        }

        public async Task PlaylistAdd(Playlist playlist)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.ExecuteAsync("INSERT INTO Playlist(title, description) VALUES(@Title, @Description)", playlist);
            }
        }
    }
}
