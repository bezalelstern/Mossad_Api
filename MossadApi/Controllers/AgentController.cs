﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MossadApi.DAL;

using Microsoft.EntityFrameworkCore;
using MossadApi.Models;

namespace MossadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class agentController : ControllerBase
    {
        private readonly Icalculatlocation _icalculatlocation;
        private readonly DBContext _context;
        private readonly ILogger<agentController> _logger;

        public agentController(ILogger<agentController> logger, DBContext context, Icalculatlocation icalculatlocation)
        {

            this._context = context;
            this._logger = logger;
            this._icalculatlocation = icalculatlocation;
        }
        //שרת סימולציה בלבד
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAgent([FromBody] Agents agent)
        {
            this._context.Agents.Add(agent);
            await this._context.SaveChangesAsync();
            return StatusCode
                (StatusCodes.Status201Created, new { Response = true, agent = agent });
        }

        //שרת סימולציה ומנהל בקרה
        [HttpGet]
        public async Task<IActionResult> getagents()
        {
            var attacks = await this._context.Agents.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, attacks);
        }

        //שרת סימולציה בלבד
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> putlocation(int id, Dictionary<string,int> location)
        {
            Agents agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            //agent.Location = newagent.Location;
            agent.X_axis = location["x"];
            agent.Y_axis = location["y"];
            await this._context.SaveChangesAsync();
            return StatusCode(200);

        }

        //שרת סימולציה בלבד
        [HttpPut("{id}/move")]
        public async Task<IActionResult> updatlocation(int id, [FromBody] Dictionary<string, string> move)
        {
            Agents agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            List<Mission> missions = _context.Mission.ToList();
            foreach (Mission mission in missions)
            {
                if (mission.AgentId == agent.Id)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new {messege = "the agent already in a mission"});
                }
            }
            //ביצוע הזזה במטריצה 
            agent =  _icalculatlocation.AgentLocation(agent, move);

            return StatusCode(200, new {agent = agent});

        }


    }

    


}
