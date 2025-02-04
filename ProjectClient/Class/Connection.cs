﻿using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ProjectClient.Class
{
    public class Connection
    {
        HubConnection connection;
        private const string GROUP_NAME = "Game Hub";
        private String connectionId = "";

        Form1 form;
        GameMap gameMap;

        public Connection()
        {

        }

        public void SetForm(Form1 form)
        {
            this.form = form;
        }

        public void SetGameMap(GameMap gameMap)
        {
            this.gameMap = gameMap;
        }

        public void GameMapUpdatePlayerPos(string playerX, string playerY, string connectionId, string groupName)
        {
            connection.InvokeCoreAsync("UpdatePlayerPos", args: new[] { playerX, playerY, connectionId, groupName });
        }

        public void FormJoinGroup()
        {
            connection.InvokeCoreAsync("JoinGroup", args: new[] { GROUP_NAME });
            //await connection.InvokeCoreAsync("JoinGroup", args: new[] { GROUP_NAME });
        }

        public void FormLeaveGroup()
        {
            connection.InvokeCoreAsync("RemoveFromGroup", args: new[] { GROUP_NAME });
            //await connection.InvokeCoreAsync("RemoveFromGroup", args: new[] { GROUP_NAME });
        }

        public void ConnectToServer()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();

            ServerResponseHandling();

            connection.StartAsync();
            connection.InvokeCoreAsync("ConnectionTest", args: new[] { "testing connection" });

            //trying connect to server 
            //TODO: need to fix
            /*connection.Closed += async (error) =>
            {
                //txtConnection.Text = "Trying to connect to server";
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();

                //sending to server
                //await connection.SendAsync("SendMessage", new[] { "User1", "Ready" });
                await connection.InvokeCoreAsync("SendMessage", args: new[] { "User1", "Ready" });
            };*/
        }

        public void SpawnCoin(int x, int y, string groupName)
        {
            connection.InvokeCoreAsync("SpawnCoin", args: new[] { x.ToString(), y.ToString(), groupName });
        }

        public void AddPlayerPoints(int points, string connectionId, string groupName)
        {
            connection.InvokeCoreAsync("AddPlayerPoints", args: new[] { points.ToString(), connectionId, groupName });
        }

        public void SpawnSpecialItem(int x, int y, int id, string playerConnection, string groupName)
        {
            connection.InvokeCoreAsync("SpawnSpecialItem", args: new[] { x.ToString(), y.ToString(), id.ToString(), playerConnection, groupName });
        }

        public void PlaceItem(int x, int y, int itemId, string connectionId, string groupName)
        {
            connection.InvokeCoreAsync("PlaceItem", args: new[] { x.ToString(), y.ToString(), itemId.ToString(), connectionId, groupName });
        }

        public void AddPlayerItem(int itemId, string connectionId, string groupName)
        {
            connection.InvokeCoreAsync("AddPlayerItem", args: new[] { itemId.ToString(), connectionId, groupName });
        }

        public void RemovePlayerItem(string connectionId, string groupName)
        {
            connection.InvokeCoreAsync("RemovePlayerItem", args: new[] {connectionId, groupName });
        }

        public void SetTextColor(string color, string groupName)
        {
            connection.InvokeCoreAsync("SetTextColor", args: new[] {color, groupName});
        }

        private void ServerResponseHandling()
        {
            connection.On("UpdatePlayers", (int X, int Y, string connectionId, string groupName) =>
            {
                //remove 
                connection.InvokeCoreAsync("ConnectionTest", args: new[] { "conenction UpdatePlayers" });
                //connection.InvokeCoreAsync("ConnectionTest", args: new[] { "conenction UpdatePlayers" });
                gameMap.UpdatePlayerByServer(X, Y, connectionId);
            });

            connection.On("SpawnCoin", (int X, int Y) =>
            {
                gameMap.SpawnCoinByServer(X, Y);
            });

            connection.On("SetTextColor", (string color) =>
            {
                gameMap.SetTextColorByServer(color);
            });

            connection.On("AddPlayerPoints", (int points, string connectionId) =>
            {
                gameMap.AddPlayerPointsByServer(points, connectionId);
            });

            connection.On("PlaceItem", (int X, int Y, int itemId, string connectionId) =>
            {
                gameMap.PlaceItemByServer(X, Y, itemId, connectionId);
            });

            connection.On("SpawnSpecialItem", (int X, int Y, int id, string playerConnection) =>
            {
                gameMap.SpawnSpecialItemByServer(X, Y, id, playerConnection);
            });

            connection.On("AddPlayerItem", (int itemId, string connectionId) =>
            {
                gameMap.AddPlayerItemByServer(itemId, connectionId);
            });

            connection.On("RemovePlayerItem", (string connectionId) =>
            {
                gameMap.RemovePlayerItemByServer(connectionId);
            });

            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                form.ReceiveMessage(userName, message);
            });

            connection.On("ConnectionTest", (string test) =>
            {
                form.ConnectionTest(test);
            });

            connection.On("JoinedGroupUpdateId", (string ConnectionId) =>
            {
                connectionId = ConnectionId;
            });

            connection.On("JoinedGroup", (string info, string ConnectionId) =>
            {
                form.JoinedGroup(info);
            });

            connection.On("RemoveFromGroup", (string info) =>
            {
                form.RemoveFromGroup(info);
                connectionId = "";
            });

            connection.On("StartGroupGameSession", (string info, List<string> groupMemebers) =>
            {
               gameMap = new GameMap(groupMemebers, GROUP_NAME, connectionId, this);
                gameMap.Show();
               //gameMap.ShowDialog();

               //remove 
               connection.InvokeCoreAsync("ConnectionTest", args: new[] { "conenction UpdatePlayers" });
            });

            connection.On("GroupError", (string info) =>
            {
                form.GroupError(info);
            });
        }
    }
}
