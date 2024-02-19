using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using AutoMapper;
using HotelListing.API.Contracts;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    { 
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        /* public CountriesController(HotelListingDbContext context)
         {
             _context = context;
         }*/

        //injection of the automapper
        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
        {
            this._mapper = mapper;
            this._countriesRepository = countriesRepository;
        }

        // GET: api/Countries
        [HttpGet]
        /*public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            //return await _context.Countries.ToListAsync();

            int countries = await _context.Countries.ToListAsync();
            return Ok(countries);
        }*/

        //to avoid the giving list of countries just giving list of getcountrydto
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            //return await _context.Countries.ToListAsync();

            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);//mapping list of countries to one object of country
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            // var country = await _context.Countries.FindAsync(id);

            //var country = await _context.Countries.Include(q => q.Hotels)
                //.FirstOrDefaultAsync(q => q.Id == id);//to refactor it to include hotels details into country dto

            var country = _countriesRepository.GetDetails(id);
            
            if (country == null)
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(country);


            //return country;
            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //public async Task<IActionResult> PutCountry(int id, Country country)//id of the country, country object

        //for update country dto
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid Country Id");
            }

            //// _context.Entry(country).State = EntityState.Modified;
            ///
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            _mapper.Map(updateCountryDto, country);//assign values from leftside var to right 
          

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        /*public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }*/



        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            /*var countryOld = new Country
            {
                Name = createCountryDto.Name,
                ShortName = createCountryDto.ShortName,
            };*/

            var country = _mapper.Map<Country>(createCountryDto);//one country mapped to another (one to one)

            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            // _context.Countries.Remove(country);
            //await _context.SaveChangesAsync();
            //above lines will be replaces by the below one
            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            //return _context.Countries.Any(e => e.Id == id);
            return await _countriesRepository.Exists(id);
        }
    }
}
