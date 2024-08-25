﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MossadApi.DAL;
using MossadApi.Models;
namespace MossadApi.Controllers
{
    [Route("/[controller]s")]
    [ApiController]
    public class targetController : ControllerBase
    {
        
        private readonly Icalculatlocation _icalculatlocation;
        private readonly ISetmission _setmission;
        private readonly DBContext _context;
        private readonly ILogger<targetController> _logger;

        public targetController(ILogger<targetController> logger, DBContext context, Icalculatlocation icalculatlocation, ISetmission setmission)
        {

            this._context = context;
            this._logger = logger;
            this._icalculatlocation = icalculatlocation;
            this._setmission = setmission;
           
        }

      
        //שרת סימולציה בלבד
        //איתחול מיקום המטרה
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> putlocation(int id, Dictionary<string, int> location)
        {
           Target target = await _context.Targets.FindAsync(id);
            if (target == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            target.X_axis = location["x"];
            target.Y_axis = location["y"];
            await this._context.SaveChangesAsync();
            return StatusCode(200);

        }

        //שרת סימולציה בלבד
        //יוצר מטרה
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> createtarget([FromBody] Target target)
        {
            this._context.Targets.Add(target);
            await this._context.SaveChangesAsync();

            _setmission.Set();
            return StatusCode
                (StatusCodes.Status201Created, target );
        }

        //גישה חופשית
        //יוצר רשימת מטרות
        [HttpGet]
        public async Task<IActionResult> gettargets()
        {
            List<Target> targets = await _context.Targets.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, targets);
        }




        //שרת סימולציה בלבד
        //מבצע תזוזה
        [HttpPut("{id}/move")]
        public async Task<IActionResult> updatlocation(int id, [FromBody] Dictionary<string, string> move)
        {
            Target target = await _context.Targets.FindAsync(id);
            if (target == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (target.Alive == false)
            {
                return BadRequest();
            }

            target = await _icalculatlocation.TargetLocation(target, move);
            await _context.SaveChangesAsync();
            return StatusCode(200, new { target = target });

        }
    }
}
