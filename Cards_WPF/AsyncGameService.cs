using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards;

namespace Cards_WPF
{
    class AsyncGameService
    {
        private readonly GameService gameService;

        private BackgroundWorker backgroundWorker = new BackgroundWorker();


        public AsyncGameService(GameService gameService)
        {
            this.gameService = gameService;
        }

        public void CreateRound(int n)
        {
            Task.Run(() => gameService.CreateRound(n));
        }
    }
}
