using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotGames.Help2Swagger
{
    public class HelpFullSchema
    {
        public HelpFullEventSchema[] Events { get; set; }
        public HelpFullFunctionSchema[] Functions { get; set; }
        public HelpFullTypeSchema[] Types { get; set; }
    }
}
