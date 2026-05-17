using System;

namespace VirtualPlantCareAssistantVSC.Models
{
    public class Plant
    {
        public int PlantId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int WateringFrequencyDays { get; set; }
        public DateTime? LastWatered { get; set; }
        public int FertilizationFrequencyDays { get; set; }
        public DateTime? LastFertilized { get; set; }
        public string ImagePath { get; set; }

        // --- Computed Properties ---
        public DateTime? NextWateringDate => LastWatered?.AddDays(WateringFrequencyDays);
        public DateTime? NextFertilizationDate => LastFertilized?.AddDays(FertilizationFrequencyDays);

        public string WateringStatus
        {
            get
            {
                if (!NextWateringDate.HasValue) return "Due";
                var diff = (NextWateringDate.Value.Date - DateTime.Today).TotalDays;
                if (diff < 0) return "Overdue";
                if (diff < 1) return "Due Today";
                return "OK";
            }
        }

        public string FertilizationStatus
        {
            get
            {
                if (!NextFertilizationDate.HasValue) return "Due";
                var diff = (NextFertilizationDate.Value.Date - DateTime.Today).TotalDays;
                if (diff < 0) return "Overdue";
                if (diff < 1) return "Due Today";
                return "OK";
            }
        }

        public string WateringStatusColor
        {
            get
            {
                switch (WateringStatus)
                {
                    case "Overdue": return "#F44336";
                    case "Due Today": return "#FF9800";
                    case "Due": return "#FF9800";
                    default: return "#4CAF50";
                }
            }
        }

        public string FertilizationStatusColor
        {
            get
            {
                switch (FertilizationStatus)
                {
                    case "Overdue": return "#F44336";
                    case "Due Today": return "#FF9800";
                    case "Due": return "#FF9800";
                    default: return "#8BC34A";
                }
            }
        }

        public string NextWateringText => NextWateringDate.HasValue
            ? $"Next: {NextWateringDate.Value:MMM dd}"
            : "Not watered yet";

        public string NextFertilizationText => NextFertilizationDate.HasValue
            ? $"Next: {NextFertilizationDate.Value:MMM dd}"
            : "Not fertilized yet";

        public Plant() { }

        public Plant(int plantId, int userId, string name, string species,
            int wateringFrequencyDays, DateTime? lastWatered,
            int fertilizationFrequencyDays, DateTime? lastFertilized,
            string imagePath)
        {
            PlantId = plantId;
            UserId = userId;
            Name = name;
            Species = species;
            WateringFrequencyDays = wateringFrequencyDays;
            LastWatered = lastWatered;
            FertilizationFrequencyDays = fertilizationFrequencyDays;
            LastFertilized = lastFertilized;
            ImagePath = imagePath;
        }
    }
}
