using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using ScenarioTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClickTournament.E2ETests
{
    public partial class PlaywrightExample
    {
        [Scenario]
        public async Task Scenario1(ScenarioContext scenario)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://playwright.dev/dotnet");

            scenario.Fact("Sample1", () => Assert.NotNull(page));
        }                
    }
}
