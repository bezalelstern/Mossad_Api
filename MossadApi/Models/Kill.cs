﻿namespace MossadApi.Models
{
    public class Kill
    {
        public int AgentId { get; set; }

        public int TargetId { get; set; }

        public DateTime MissionTime { get; set; }
    }
}
