using OpenQA.Selenium.Chrome;
using ScenarioTests;
using System;
using Xunit;

namespace ClickTournament.E2ETests
{
    public partial class SeleniumExample
    {
        [Scenario]
        public void PlayScenario(ScenarioContext scenario)
        {
            using var driver = new ChromeDriver(".")
            {
                Url = "http://the-internet.herokuapp.com/"
            };

            scenario.Fact("Title as expected", () => Assert.True(driver.Title == "The Internet"));
        }
    }
}
