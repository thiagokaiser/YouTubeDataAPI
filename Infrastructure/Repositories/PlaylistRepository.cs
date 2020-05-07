using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Dapper;
using Npgsql;
using System.Linq;
using Core.ViewModels;

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

        public async Task<Playlist> PlaylistVideosById(int id)
        {            
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var query = @"
                    SELECT * FROM playlist 
                    LEFT JOIN videoplaylist ON videoplaylist.playlistid = playlist.id 
                    LEFT JOIN video ON video.id = videoplaylist.videoid
                    WHERE playlist.id = @Id;
                    ";

                var videos = await conexao.QueryAsync<Playlist, Video, Playlist>(query,
                    (playlist, video) =>
                    {
                        playlist.Videos = playlist.Videos ?? new List<Video>();
                        playlist.Videos.Add(video);
                        return playlist;
                    }, splitOn: "playlistid", param: new { Id = id });

                var grouped = videos.GroupBy(o => o.Id)
                    .Select(group =>
                    {
                        var combinedPlaylist = group.First();
                        combinedPlaylist.Videos = group.Select(playlist => playlist.Videos.Single()).ToList();
                        return combinedPlaylist;
                    });                

                return grouped.First();                
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

        public async Task RemoveVideoFromPlaylist(VideoPlaylistViewModel videoPlaylist)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var playlist = await conexao.ExecuteAsync(@"DELETE FROM videoplaylist                                                          
                                                             WHERE videoid = @VideoId AND playlistid = @PlaylistId", videoPlaylist);
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
