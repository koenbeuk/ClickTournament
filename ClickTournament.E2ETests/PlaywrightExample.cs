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
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public async Task Scenario1(ScenarioContext scenario)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

            // Launch a new page navigating to Localhost (assuming project is running)
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://localhost:5001");
            
            // Test if we were able to load in the title correctly
            await scenario.Fact("Page has a correct title", async () =>
            {
                Assert.Equal("ClickTournament", await page.TitleAsync());
            });

            // Wait for our app to load and expect a play button 
            var playButton = await page.WaitForSelectorAsync("button");

            // Assert that we're dealing with a play button
            await scenario.Fact("Page is in the initial state", async () =>
            {
                Assert.Equal("play!", await playButton.TextContentAsync());
            });

            // Trigger the start of a game session
            await playButton.ClickAsync();

            // Expect a level div to be generated for us
            var levelElement = await page.WaitForSelectorAsync(".level");

            // Assert that we indeed got a level div
            await scenario.Fact("Page has transitioned to the Starting state", async () =>
            {
                Assert.StartsWith("Starting in:", await levelElement.TextContentAsync());
            });

            // Wait for the game to start
            var targetElement = await page.WaitForSelectorAsync(".target");

            // Assert that we received a target
            await scenario.Fact("Page has transitioned to the Playing state", async () =>
            {
                Assert.NotNull(targetElement);
            });

            // Perform a hit click
            await targetElement.ClickAsync();
            // Perform a miss click
            await levelElement.ClickAsync();

            // Wait for the game to be over and the scoreboard to popup
            var scoreboardElement = await page.WaitForSelectorAsync(".card");

            // Assert that we're correctly dealing with a scoreboard
            await scenario.Fact("Page has transitioned to the Finished state", async () =>
            {
                var titleElement = await scoreboardElement.QuerySelectorAsync("h2");
                Assert.Equal("Congrats!", await titleElement.TextContentAsync());
            });
            // Assert that we are correctly showing our 1 hit
            await scenario.Fact("Page has recorded our hit click", async () =>
            {
                var missesLabel = await scoreboardElement.QuerySelectorAsync("p");
                Assert.Equal("Hits: 1", await missesLabel.TextContentAsync());
            });
            // Assert that we're correctly showing our 1 miss
            await scenario.Fact("Page has recorded our miss click", async () =>
            {
                var missesLabel = await scoreboardElement.QuerySelectorAsync("p + p");
                Assert.Equal("Misses: 1", await missesLabel.TextContentAsync());
            });
        }
    }
}
