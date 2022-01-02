using System;

namespace TrackCleaning.JSONElements
{
    /// <summary>
    /// Class which stores all information about on Cleaning-Procedure. This data will be written into the AAS. 
    /// </summary>
    class CleaningProcedure
    {
        /// <summary>
        /// The id of the Cleaning-Procedure. This is an auto-increment number starting at 1. 
        /// </summary>
        public string CleaningID { get; set; }

        /// <summary>
        /// The id of the used CIP-Module. 
        /// </summary>
        public string CIPID { get; set; }

        /// <summary>
        /// The id of the Object which was cleaned. 
        /// </summary>
        public string CleaningObjectID { get; set; }

        /// <summary>
        /// The date of the started Cleaning-Procedure.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The StartingTime of the Cleaning-Procedure. 
        /// </summary>
        public TimeSpan StartingTime { get; set; }

        /// <summary>
        /// The EndingTime of the Cleaning-Procedure. 
        /// </summary>
        public TimeSpan EndingTime { get; set; }

        /// <summary>
        /// The duration of whole Cleaning-Procedure. 
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// A bool whether the Cleaning-Procedure is manual intervented or not.
        /// </summary>
        public bool IsCleaningSuccessfulCompleted { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: StartingTime of this specific step. 
        /// </summary>
        public TimeSpan PreRinseStartingTime { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: EndingTime of this specific step. 
        /// </summary>
        public TimeSpan PreRinseEndingTime { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: Prescriped WaterAmount of this specific step. 
        /// </summary>
        public string PreRinseWaterAmount { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: Prescriped Time of this specific step. 
        /// </summary>
        public string PreRinseTime { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: Prescriped WaterAmount of this specific step. 
        /// </summary>
        public string PreRinseConductivity { get; set; }

        /// <summary>
        /// PreRinse-Cleaning-Phase: Prescriped Temperature of this specific step. 
        /// </summary>
        public string PreRinseTemperature { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: StartingTime of this specific step. 
        /// </summary>
        public TimeSpan CausticStartingTime { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: EndingTime of this specific step. 
        /// </summary>
        public TimeSpan CausticEndingTime { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: Prescriped WaterAmount of this specific step. 
        /// </summary>
        public string CausticWaterAmount { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: Prescriped WaterAmount of this specific step. 
        /// </summary>
        public string CausticTime { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: Prescriped Conductivity of this specific step. 
        /// </summary>
        public string CausticConductivity { get; set; }

        /// <summary>
        /// Caustic-Cleaning-Phase: Prescriped Temperature of this specific step. 
        /// </summary>
        public string CausticTemperature { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: StartingTime of this specific step. 
        /// </summary>
        public TimeSpan IntermediateRinseStartingTime { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: EndingTime of this specific step. 
        /// </summary>
        public TimeSpan IntermediateRinseEndingTime { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: Prescribed WaterAmount of this specific step. 
        /// </summary>
        public string IntermediateRinseWaterAmount { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: Prescribed Time of this specific step. 
        /// </summary>
        public string IntermediateRinseTime { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: Prescribed Conductivity of this specific step. 
        /// </summary>
        public string IntermediateRinseConductivity { get; set; }

        /// <summary>
        /// IntermediateRinse-Cleaning-Phase: Prescribed Temperature of this specific step. 
        /// </summary>
        public string IntermediateRinseTemperature { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: StartingTime of this specific step. 
        /// </summary>
        public TimeSpan AcidStartingTime { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: EndingTime of this specific step. 
        /// </summary>
        public TimeSpan AcidEndingTime { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: Prescribed WaterAmount of this specific step. 
        /// </summary>
        public string AcidWaterAmount { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: Prescribed Time of this specific step. 
        /// </summary>
        public string AcidTime { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: Prescribed Conductivity of this specific step. 
        /// </summary>
        public string AcidConductivity { get; set; }

        /// <summary>
        /// Acid-Cleaning-Phase: Prescribed Temperature of this specific step. 
        /// </summary>
        public string AcidTemperature { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: StartingTime of this specific step. 
        /// </summary>
        public TimeSpan FinalRinseStartingTime { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: EndingTime of this specific step. 
        /// </summary>
        public TimeSpan FinalRinseEndingTime { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: Prescribed WaterAmount of this specific step. 
        /// </summary>
        public string FinalRinseWaterAmount { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: Prescribed Time of this specific step. 
        /// </summary>
        public string FinalRinseTime { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: Prescribed Conductivity of this specific step. 
        /// </summary>
        public string FinalRinseConductivity { get; set; }

        /// <summary>
        /// FinalRinse-Cleaning-Phase: Prescribed Temperature of this specific step. 
        /// </summary>
        public string FinalRinseTemperature { get; set; }


    }
}
