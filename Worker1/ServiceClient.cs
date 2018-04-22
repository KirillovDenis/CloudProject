using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DDEvernote.WPF
{
    public class ServiceClient
    {
        private readonly HttpClient _client;

        public ServiceClient(string connectionString)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionString);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public User CreateUser(User user)
        {
            user = _client.PostAsJsonAsync("users", user).Result.Content.ReadAsAsync<User>().Result;
            return user;
        }




        public User GetUser(Guid userId)
        {
            return _client.GetAsync($"users/{userId}").Result.Content.ReadAsAsync<User>().Result;
        }

        public User GetUserByName(String userName)
        {
            var user = new User();
            user = _client.GetAsync($"users/name/{userName}").Result.Content.ReadAsAsync<User>().Result;
            return user;
        }

        public Note CreateNote(Note note)
        {
            note = _client.PostAsJsonAsync("notes", note).Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }

        public Note GetNote(Guid noteId)
        {
            Note note;
            note = _client.GetAsync($"notes/{noteId}").Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }

        public IEnumerable<Note> GetNotes(Guid userId)
        {
            return _client.GetAsync($"users/{userId}/notes").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }

        public IEnumerable<Note> GetNotesBySharedUser(Guid ownerUserId, Guid sharedUserId)
        {
            return _client.GetAsync($"users/{ownerUserId}/shared/{sharedUserId}").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }


        public Note UpdateNote(Note note)
        {
            return _client.PutAsJsonAsync("notes", note).Result.Content.ReadAsAsync<Note>().Result;
        }


        public void DeleteNote(Guid noteId)
        {
            _client.DeleteAsync($"notes/{noteId}");
        }

        
    }
}
