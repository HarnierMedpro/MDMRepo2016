using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Controllers.WebAPI
{
    public class WSCorp_DBsController : ApiController
    {
        private MedProDBEntities db = new MedProDBEntities();

        // GET: api/WSCorp_DBs
        public IQueryable<Corp_DBs> GetCorp_DBs()
        {
            return db.Corp_DBs;
        }

        // GET: api/WSCorp_DBs/5
        [ResponseType(typeof(Corp_DBs))]
        public async Task<IHttpActionResult> GetCorp_DBs(int id)
        {
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            if (corp_DBs == null)
            {
                return NotFound();
            }

            return Ok(corp_DBs);
        }

        // PUT: api/WSCorp_DBs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCorp_DBs(int id, Corp_DBs corp_DBs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != corp_DBs.ID_PK)
            {
                return BadRequest();
            }

            db.Entry(corp_DBs).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Corp_DBsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WSCorp_DBs
        [ResponseType(typeof(Corp_DBs))]
        public async Task<IHttpActionResult> PostCorp_DBs(Corp_DBs corp_DBs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Corp_DBs.Add(corp_DBs);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = corp_DBs.ID_PK }, corp_DBs);
        }

        // DELETE: api/WSCorp_DBs/5
        [ResponseType(typeof(Corp_DBs))]
        public async Task<IHttpActionResult> DeleteCorp_DBs(int id)
        {
            Corp_DBs corp_DBs = await db.Corp_DBs.FindAsync(id);
            if (corp_DBs == null)
            {
                return NotFound();
            }

            db.Corp_DBs.Remove(corp_DBs);
            await db.SaveChangesAsync();

            return Ok(corp_DBs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Corp_DBsExists(int id)
        {
            return db.Corp_DBs.Count(e => e.ID_PK == id) > 0;
        }
    }
}