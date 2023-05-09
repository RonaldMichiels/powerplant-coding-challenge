using PowerplantApp.Entities;
using System.Security.Cryptography.X509Certificates;

namespace PowerplantApp.Services
{
    public class PowerLoadService
    {
        // Basics

        // Gas or Kerosine
        // Price =      6
        // Efficiency = 0.50 (-> 2 units of gas = 1 unit of Electricity)
        // Cost =       12
        
        // formula -> price / efficiency = cost

        // Wind

        // Wind Percentage = 0.25
        // PMax =            4

        // formula -> Pmax * Wind Percentage


        // Receive

        // - load
        // - fuels and price
        // - available powerplants, their efficiencies and min and max power.

        // OUTPUT tells us how much power each powerplant from the INPUT should produce.
        // SUM of all POWER produced by the powerplants should equal the load.


        // 3 Types of powerplants
        // - Windturbine - FUEL = WIND --> 100% efficiency but relies on wind
        // - Turbojet - FUEL = KEROSINE --> low efficiency and expensive but ability to produce 0
        // - Gasfired - FUEL = GAS --> Decent efficiency, relatively expensive but high power output though minimum power output as well

        // Check wind first, then gas fired, then turbo
        // lowest cost possible for exact power needed

        public static EnergyPlanResponse CalculatePower(EnergyPlanPayload payload)
        {

            // loop over every powerplant
            // decide the type 
            // calculate an effectiveness score depending on the fuel + type
            // rank them from super cost effective to lower cost effectiveness
            // calculate accordingly power + cost

            // Cost Effectiveness = PMax Power * Efficiency / Cost in eur


            // create report

            var powerplants = FindOptimalDistribution(payload.powerplants, payload.load, payload.fuels);
            var energyResults = powerplants.Select(powerplant => new EnergyResult(powerplant.name, powerplant.pcurrent)).ToList();

            var energyPlanResponse = new EnergyPlanResponse();
            energyPlanResponse.EnergyResults = energyResults;
            return energyPlanResponse;
        }

        private static float CalculatePricePerUnitOfElectricty(Powerplant powerplant, Fuel fuel)
        {
            // COST EFFECTIVENESS = price per unit of electricity
            // take price 1UF (unit of fuel) * efficiency % = price per 1 UE (unit of electricity)
            // for wind -> cost is zero
            // price per unit of electricity is cost effectiveness
            // rank order ascending (lowest price is better)

            // BUT take into account pmin ? -> in selection. Not here

            float pricePerUnitOfElectricity = 0;
            if (powerplant.type == "turbojet")
            {
                pricePerUnitOfElectricity = (fuel.kerosine * 1.0f) / powerplant.efficiency;
            } 
            else if (powerplant.type == "gasfired")
            {
                pricePerUnitOfElectricity = (fuel.gas * 1.0f) / powerplant.efficiency;
            } 
            return pricePerUnitOfElectricity;
        }

        private static List<Powerplant> FindOptimalDistribution(List<Powerplant> powerplants, int load, Fuel fuel)
        {
            foreach (var powerplant in powerplants)
            {
                if (powerplant.type == "windturbine")
                {
                    powerplant.pmax = powerplant.pmax * (fuel.wind / 100);
                    powerplant.typeCategory = 2;
                }
                powerplant.pricePerUnitOfElectricity = CalculatePricePerUnitOfElectricty(powerplant, fuel);
            }

            var sortedPowerplants = powerplants.OrderBy(powerplant => powerplant.pricePerUnitOfElectricity).ToList();

            var remainingLoad = load;
            var powerSum = 0;

            var windPlants = sortedPowerplants.Where(powerplant => powerplant.type == "windturbine").ToList();
            var gasPlants = sortedPowerplants.Where(powerplant => powerplant.type == "gasfired").ToList();
            var jetPlants = sortedPowerplants.Where(powerplant => powerplant.type == "turbojet").ToList();

            var result = new List<Powerplant>();

            // ask current cost (remaining power) of power stations -> lowest cost -> select if remaining power does not exceed pmin of remaining unused power stations
            // pop power station
            // for remaining power -> do the same

            // if sum > load -> check difference. Check if wind power

            // the issue is ofcourse we need to match the load exactly at the lowest cost
            // right now we take the max power, min power, remaing power or no power. We don't distribute the power evenly for example.

            var gasPMinSum = gasPlants.Sum(gasPlant => gasPlant.pmin);
            gasPlants.ForEach(gasplant => gasplant.pcurrent = gasplant.pmin);
            remainingLoad = remainingLoad - gasPMinSum;

            foreach (var powerplant in gasPlants)
            {
                if (remainingLoad == 0)
                {
                    break;
                }
                if (remainingLoad + powerplant.pmin > powerplant.pmax)
                {
                    powerplant.pcurrent = powerplant.pmax;
                    remainingLoad = (remainingLoad + powerplant.pmin) - powerplant.pcurrent;
                }
                else if (remainingLoad + powerplant.pmin <= powerplant.pmax)
                {
                    powerplant.pcurrent = powerplant.pmin + remainingLoad;
                    remainingLoad = 0;
                }
                powerSum += powerplant.pcurrent;
            }

            // check if wind power
            if (fuel.wind > 0)
            {
                foreach (var powerplant in windPlants)
                {
                    if (remainingLoad == 0)
                    {
                        break;
                    }
                    if (remainingLoad >= powerplant.pmax)
                    {
                        powerplant.pcurrent = powerplant.pmax;
                        remainingLoad = remainingLoad - powerplant.pcurrent;
                    }
                    else
                    {
                        powerplant.pcurrent = remainingLoad;
                        remainingLoad = remainingLoad - powerplant.pcurrent;
                    }
                    powerSum += powerplant.pcurrent;

                }
            }

            // check if we can replace fossil power with wind power
            // is all windpower used ?

            foreach (var powerplant in jetPlants)
            {
                if (remainingLoad == 0)
                {
                    break;
                }
                if (remainingLoad >= powerplant.pmax)
                {
                    powerplant.pcurrent = powerplant.pmax;
                    remainingLoad = remainingLoad - powerplant.pcurrent;
                } 
                else
                {
                    powerplant.pcurrent = remainingLoad;
                    remainingLoad = remainingLoad - powerplant.pcurrent;
                }
                powerSum += powerplant.pcurrent;
            }

            // check remaining load and sum

            result.AddRange(windPlants);
            result.AddRange(gasPlants);
            result.AddRange(jetPlants);
            return result;
        }

    }
}
