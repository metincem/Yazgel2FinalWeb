using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YazGelFinal.WebUI.EFCore;
using YazGelFinal.WebUI.Models;

namespace YazGelFinal.WebUI.Hubs
{
    public class GameHub : Hub
    {
        EFCoreContext efContext;
        private readonly IHubContext<GameHub> hubContext;
        public GameHub(IHubContext<GameHub> _hubContext)
        {
            efContext = new EFCoreContext();
            hubContext = _hubContext;
        }

        public void GameStarted()
        {
            string id = Context.ConnectionId;
            new Thread(() => OnWaitEnded(id)).Start();
        }

        private void OnWaitEnded(string id)
        {
            Thread.Sleep(241000);
            hubContext.Clients.Client(id).SendAsync("Status", "/Routing/" + System.Web.HttpUtility.UrlEncode(id));
        }

        public async Task SendGuid(string guid)
        {
            GameProccess procces = efContext.GameProccesses.FirstOrDefault(x => x.ImageRandomGuid.Equals(guid) && x.ConnectionId.Equals(Context.ConnectionId));
            GameProccess epr = efContext.GameProccesses.FirstOrDefault(x => x.WasShown && x.ConnectionId.Equals(Context.ConnectionId));
            if (epr != null)
            {
                if (procces.ImagePath.Equals(epr.ImagePath))
                {
                    epr.WasShown = false;
                    procces.WasShown = false;
                    epr.Completed = true;
                    procces.Completed = true;
                    efContext.Update(epr);
                    efContext.Update(procces);
                    efContext.SaveChanges();
                    await Clients.Caller.SendAsync("cmpltd", epr.CardLocation, procces.CardLocation, 1, "/CardImages/" + procces.ImagePath);
                    int cmpltdCount = efContext.GameProccesses.Where(x => x.ConnectionId.Equals(Context.ConnectionId) && x.Completed && x.CardLevel == procces.CardLevel).Select(x => new { x.Id }).Count();
                    if (cmpltdCount == 24)
                    {
                        string url = "/Routing/" + System.Web.HttpUtility.UrlEncode(procces.ConnectionId);
                        await Clients.Caller.SendAsync("Status", "/Routing/" + System.Web.HttpUtility.UrlEncode(Context.ConnectionId));
                        return;
                    }
                    if (cmpltdCount == (procces.CardLevel + 1) * 6)
                    {
                        Thread.Sleep(1800);

                        List<GameProccess> prcList = efContext.GameProccesses.Where(x => x.ConnectionId.Equals(Context.ConnectionId) && x.CardLevel == (procces.CardLevel + 1)).OrderBy(x => x.CardLocation).ToList();

                        string[] cards = new string[(procces.CardLevel + 2) * 6];
                        for (int i = 0; i < (procces.CardLevel + 2) * 6; i++)
                            cards[i] = prcList[i].ImageRandomGuid;

                        await Clients.Caller.SendAsync("receiveCards", cards);
                    }
                    return;
                }
                else
                {
                    epr.WasShown = false;
                    procces.WasShown = false;
                    efContext.Update(epr);
                    efContext.Update(procces);
                    efContext.SaveChanges();
                    await Clients.Caller.SendAsync("cmpltd", epr.CardLocation, procces.CardLocation, 0, "/CardImages/" + procces.ImagePath);
                    return;
                }
            }
            procces.WasShown = true;
            efContext.GameProccesses.Update(procces);
            efContext.SaveChanges();
            string imagePath = "/CardImages/" + procces.ImagePath;
            await Clients.Caller.SendAsync("receiveSv", imagePath, procces.CardLocation);
        }

        public async override Task OnConnectedAsync()
        {
            List<string> shuffled = new List<string>() { "Flower", "Animal", "Emoji" }.OrderBy(x => Guid.NewGuid()).ToList();
            List<Card> level1 = efContext.Cards.Where(x => x.Theme.Equals(shuffled[0])).OrderBy(x => Guid.NewGuid()).Take(6).ToList();
            List<Card> level2 = efContext.Cards.Where(x => x.Theme.Equals(shuffled[1])).OrderBy(x => Guid.NewGuid()).Take(9).ToList();
            List<Card> level3 = efContext.Cards.Where(x => x.Theme.Equals(shuffled[2])).OrderBy(x => Guid.NewGuid()).Take(12).ToList();

            level1.AddRange(level1);
            level2.AddRange(level2);
            level3.AddRange(level3);

            level1 = level1.OrderBy(x => Guid.NewGuid()).ToList();
            level2 = level2.OrderBy(x => Guid.NewGuid()).ToList();
            level3 = level3.OrderBy(x => Guid.NewGuid()).ToList();

            List<GameProccess> pr1 = new List<GameProccess>();
            List<GameProccess> pr2 = new List<GameProccess>();
            List<GameProccess> pr3 = new List<GameProccess>();
            for (int i = 0; i < level1.Count; i++)
            {
                pr1.Add(new GameProccess()
                {
                    CardLevel = 1,
                    CardLocation = i,
                    ConnectionId = Context.ConnectionId,
                    Completed = false,
                    WasShown = false,
                    ImagePath = level1[i].Image,
                    ImageRandomGuid = Guid.NewGuid().ToString()
                });
            }
            for (int i = 0; i < level2.Count; i++)
            {
                pr2.Add(new GameProccess()
                {
                    CardLevel = 2,
                    CardLocation = i,
                    ConnectionId = Context.ConnectionId,
                    Completed = false,
                    WasShown = false,
                    ImagePath = level2[i].Image,
                    ImageRandomGuid = Guid.NewGuid().ToString()
                });
            }
            for (int i = 0; i < level3.Count; i++)
            {
                pr3.Add(new GameProccess()
                {
                    CardLevel = 3,
                    CardLocation = i,
                    ConnectionId = Context.ConnectionId,
                    Completed = false,
                    WasShown = false,
                    ImagePath = level3[i].Image,
                    ImageRandomGuid = Guid.NewGuid().ToString()
                });
            }
            efContext.GameProccesses.AddRange(pr1);
            efContext.GameProccesses.AddRange(pr2);
            efContext.GameProccesses.AddRange(pr3);
            efContext.SaveChanges();
            string[] cards = new string[12];
            for (int i = 0; i < 12; i++)
                cards[i] = pr1[i].ImageRandomGuid;

            await Clients.Caller.SendAsync("receiveCards", cards);
        }
    }
}
