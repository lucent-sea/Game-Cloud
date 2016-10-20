﻿using Game_Cloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Game_Cloud
{
    public static class Services
    {
#if DEBUG
        public static string ServicePath = "http://localhost:10677/Services/GameCloud";
#else
        public static string ServicePath = "https://translucency.info/Services/GameCloud";
#endif
        public static async Task<HttpResponseMessage> POSTContent(Request Content)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            var response = await client.PostAsync(Services.ServicePath, new StringContent(JsonHelper.Encode(Content)));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Error " + response.StatusCode + ": " + response.ReasonPhrase);
            }
            return response;
        }
        public static async Task<HttpResponseMessage> CheckAccount()
        {
            var content = new Request()
            {
                Command = "CheckAccount",
            };
            var response = await POSTContent(content);
            if (response.IsSuccessStatusCode)
            {
                VMTemp.Current.AuthenticationCode = await response.Content.ReadAsStringAsync();
            }
            return response;
        }
        public static async Task<HttpResponseMessage> CreateAccount(string NewAccountName, string HashedPassword)
        {
            var content = new Request()
            {
                AccountName = NewAccountName,
                AccountPassword = HashedPassword,
                Command = "CreateAccount",
            };
            return await POSTContent(content);
        }
        public static async Task<bool> AddGame(SyncedGame SyncedGame, string LocalFilePath)
        {
            var content = new Request()
            {
                Command = "AddGameInfo",
                SyncedGame = SyncedGame
            };
            var result = await POSTContent(content);
            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            var webClient = new WebClient();
            webClient.Headers.Add("Command", "AddGame");
            webClient.Headers.Add("AuthenticationCode", VMTemp.Current.AuthenticationCode);
            webClient.Headers.Add("AccountName", VM.Current.AccountInfo.AccountName);
            webClient.Headers.Add("SyncedGame", SyncedGame.Name);
            try
            {
                await webClient.UploadFileTaskAsync(ServicePath, LocalFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<HttpResponseMessage> DeleteAccount()
        {
            var content = new Request()
            {
                Command = "DeleteAccount",
            };
            return await POSTContent(content);
        }
        
        public static async Task<HttpResponseMessage> ImportGames(List<SyncedGame> GameList)
        {
            var content = new Request()
            {
                Command = "ImportGames",
                GameList = GameList,
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> RemoveGame(SyncedGame SyncedGame)
        {
            var content = new Request()
            {
                Command = "RemoveGame",
                SyncedGame = SyncedGame
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> GetAccount()
        {
            var content = new Request()
            {
                Command = "GetAccount",
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> GetGame(SyncedGame SyncedGame)
        {
            var content = new Request()
            {
                Command = "GetGame",
                SyncedGame = SyncedGame
            };
            return await POSTContent(content);
        }
        public static async Task<bool> SyncGame(SyncedGame SyncedGame, string LocalFilePath)
        {
            var content = new Request()
            {
                Command = "SyncGameInfo",
                SyncedGame = SyncedGame
            };
            var result = await POSTContent(content);
            if (!result.IsSuccessStatusCode)
            {
                return false;
            }
            var webClient = new WebClient();
            webClient.Headers.Add("Command", "AddGame");
            webClient.Headers.Add("AuthenticationCode", VMTemp.Current.AuthenticationCode);
            webClient.Headers.Add("AccountName", VM.Current.AccountInfo.AccountName);
            webClient.Headers.Add("SyncedGame", SyncedGame.Name);
            try
            {
                await webClient.UploadFileTaskAsync(ServicePath, LocalFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<HttpResponseMessage> GetKnownGames()
        {
            var content = new Request()
            {
                Command = "GetKnownGames",
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> RateGame(KnownGame KnownGame)
        {
            var content = new Request()
            {
                Command = "RateKnownGame",
                KnownGame = KnownGame,
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> AddKnownGame(KnownGame KnownGame)
        {
            var content = new Request()
            {
                Command = "AddKnownGame",
                KnownGame = KnownGame
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> GetCurrentVersion()
        {
            var content = new Request()
            {
                Command = "GetCurrentVersion",
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> ApplySubscription(string ProfileID)
        {
            var content = new Request()
            {
                Command = "ApplySubscription",
                Note = ProfileID,
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> ChangePassword (string HashedPassword)
        {
            var content = new Request()
            {
                Command = "ChangePassword",
                Note = HashedPassword,
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> UpdateRecoveryOptions()
        {
            var content = new Request()
            {
                Command = "UpdateRecoveryOptions",
                AccountInfo = VM.Current.AccountInfo,
            };
            return await POSTContent(content);
        }
        public static async Task<HttpResponseMessage> RecoverPassword(string AccountName, string Method, string Answer)
        {
            var content = new Request()
            {
                Command = "RecoverPassword",
                AccountName = AccountName,
                Note = Method + "|" + Answer,
            };
            return await POSTContent(content);
        }
    }
}