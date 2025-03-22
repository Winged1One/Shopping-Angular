using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interface;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProductList(
        [FromQuery]ProductSpecParams specParams){
        var spec = new ProductSpecification(specParams);
        return await CreatePagedResult(repo,spec,specParams.PageIndex, specParams.PageSize);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id){
        var product = await repo.GetByIdAsync(id);
        if(product == null) return NotFound();
        return product;
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product){
        repo.Add(product);
        if(await repo.SaveChangesAsync()){
            return CreatedAtAction("GetProduct", new{id = product.Id}, product);
        }
        return BadRequest("Problem in creating product");
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product){
        
        if(product.Id != id || !ProductExist(id))
            return BadRequest("Cannot update this product");
        repo.Update(product);
        if(await repo.SaveChangesAsync()){
            return NoContent();
        }
        return BadRequest("Problem in Updating the product");

    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id){
        var product = await repo.GetByIdAsync(id);
        if(product == null)
            return NotFound();
        repo.Remove(product);
        if(await repo.SaveChangesAsync()){
            return NoContent();
        }
        return BadRequest("Problem in deleting the product");

    }
    private bool ProductExist(int id){
        return repo.Exists(id);
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands(){
        var spec = new BrandListSpecification();
        return Ok(await repo.ListAsync(spec));
    }
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes(){
        var spec = new TypeListSpecification();
        return Ok(await repo.ListAsync(spec));
    }
}
