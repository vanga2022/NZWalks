using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repository;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper,
            IRegionRepository regionRepository,IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkAsync()
        {
            //fetch from database- domain walk

            var walkDomain = await walkRepository.GetAllAsync();

            //convert domain walk to dto walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walkDomain);

            //return response
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            //get walk domain object from database
            var walkDomain = await walkRepository.GetAsync(id);

            //convert domain object to dto 

            var walkDTO = mapper.Map<Models.Domain.Walk, Models.DTO.Walk>(walkDomain);
            //return ok response
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {
            //valide incomping requst

             if(!(await ValidateAddWalkAsync(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }
            //convert dto object to domain object
            var walkDomain = new Models.Domain.Walk()
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId,
            };

            //pass domain object to repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);
            //conver back domain object back to dto
            var walkDTO = new Models.DTO.Walk()
            {
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };
            //send dto responses back to client

            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]

        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id,
            [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //validate the incoming request

             if(!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }
            //convert dto object to domain object
            var walkDomain = new Models.Domain.Walk()
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId
            };
            //pass details to repository - get domain object in response

            walkDomain= await walkRepository.UpdateAsync(id, walkDomain);

            //handle null (not found)
            if(walkDomain == null)
            {
                return NotFound();
            }
            //convert back domain to dto
            var walkDTO = new Models.DTO.Walk()
            {
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };       

            //return response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAysnc(Guid id)
        {
          var walkDomain= await walkRepository.DeleteAsync(id);

            if(walkDomain==null)
            {
                return NotFound();
            }

           var walkDTO= mapper.Map<Models.DTO.Walk>(walkDomain);
            return Ok(walkDTO); 

        }
        #region private method
        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            if(addWalkRequest==null)
            {
                ModelState.AddModelError(nameof(addWalkRequest),
                    $"{nameof(addWalkRequest)} can not be empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name),
                   $"{nameof(addWalkRequest.Name)} is required");
            }

            if(addWalkRequest.Length<=0)
            {
                ModelState.AddModelError(nameof(addWalkRequest.Length),
                   $"{nameof(addWalkRequest.Length)} is greater than zero");
            }

            var region= await regionRepository.GetAsync(addWalkRequest.RegionId);

            if(region==null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId),
                   $"{nameof(addWalkRequest.RegionId)} is invalid");
            }

            var walkdifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if(walkdifficulty==null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId),
                    $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid");
            }
            if(ModelState.ErrorCount>0)
            {
                return false;
            }

            return true;
        }

         private async Task<bool> ValidateUpdateWalkAsync(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest),
                    $"{nameof(updateWalkRequest)} can not be empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Name),
                   $"{nameof(updateWalkRequest.Name)} is required");
            }

            if (updateWalkRequest.Length <= 0)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Length),
                   $"{nameof(updateWalkRequest.Length)} is greater than zero");
            }

            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);

            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
                   $"{nameof(updateWalkRequest.RegionId)} is invalid");
            }

            var walkdifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkdifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId),
                    $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }

}

