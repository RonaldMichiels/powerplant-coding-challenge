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
            var remainingPower = payload.load;
            var powerSum = 0;

            // loop over every powerplant
            // decide the type 
            // calculate an effectiveness score depending on the fuel + type
            // rank them from super cost effective to lower cost effectiveness
            // calculate accordingly power + cost

            // Cost Effectiveness = PMax Power * Efficiency / Cost in eur

            foreach (var powerplant in payload.powerplants)
            {
                powerplant.costEffectiveness = CalculateCostEffectiveness(powerplant, payload.fuels);
            }

            // sort the powerplants

            var powerplantsByCostEffectiveness = payload.powerplants.OrderBy(powerplant => powerplant.costEffectiveness).Reverse().ToList();

            // create report

            var energyPlanResponse = new EnergyPlanResponse();

            // Take into account pmin for powerstations as well.
            // if pmin > remaining -> take pmin otherwise remaining

            // also check that the sum of power does not exceed the original value

            // cost efectiveness -> redifine as cost to produce 1 unit of energy, taking into account the availability of the fuel.
            // check also if what is cheaper -> remove sum calculation cause it causes errors

            foreach (var pp in powerplantsByCostEffectiveness)
            {
                var energyResult = new EnergyResult(pp.name, 0);
                if (remainingPower > 0)
                {
                    var remainingPowerBackup = remainingPower;
                    if (remainingPower > pp.pmax)
                    {
                        energyResult.p = pp.pmax;
                        remainingPower = remainingPower - pp.pmax;

                    } else if (remainingPower > pp.pmin)
                    {
                        energyResult.p = remainingPower;
                        remainingPower = 0;
                    } else
                    {
                        energyResult.p = pp.pmin;
                        remainingPower = 0;
                    }
                    powerSum += energyResult.p;
                    // check if sum exceeds max, if so, remove this one. Fallback is always turbojet.
                    if (powerSum > payload.load)
                    {
                        powerSum -= energyResult.p;
                        energyResult.p = 0;
                        remainingPower = remainingPowerBackup;
                    }
                }
                energyPlanResponse.EnergyResults.Add(energyResult);
            }
            return energyPlanResponse;
        }

        private static float CalculateCostEffectiveness(Powerplant powerplant, Fuel fuel)
        {
            float costEffectivenessRating = 0;
            if (powerplant.type == "windturbine")
            {
                costEffectivenessRating = powerplant.pmax * (fuel.wind * 1.0f / 100);
            } else if (powerplant.type == "gasfired")
            {
                var cost = fuel.gas / powerplant.efficiency;
                costEffectivenessRating = (powerplant.pmax * 1.0f) / cost;
            } else
            {
                var cost = fuel.kerosine / powerplant.efficiency;
                costEffectivenessRating = (powerplant.pmax * 1.0f) / cost;
            }
            return costEffectivenessRating;
        }

    }
}
