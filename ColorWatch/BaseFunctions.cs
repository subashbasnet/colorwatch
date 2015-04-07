using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWatch
{
    static class BaseFunctions
    {
        /**
         *@listenData 'response of 'I' received from serial port'
         * return array of calibrationfactors 
         */
        public static String[] extractCalibrationFactor(String listenData) { 
            String lData = listenData;
            int length = lData.Length;
            String[] calibrationFactor = new String[3];
            int calFactorXPosition = lData.IndexOf("Calibrationfactor_x");
            int calFactorYPosition = lData.IndexOf("Calibrationfactor_y");
            int calFactorZPosition = lData.IndexOf("Calibrationfactor_z");
            //substring is different to that of java, second parameter is not end of string, but the length of the string
            calibrationFactor[0] = lData.Substring(calFactorXPosition + 20, (calFactorYPosition-1) - (calFactorXPosition +20));
            calibrationFactor[1] = lData.Substring(calFactorYPosition+20, (calFactorZPosition-1)-(calFactorYPosition+20));
            lData  = lData.Substring(calFactorZPosition+20);
            calibrationFactor[2] = lData.Substring(0,lData.IndexOf(">"));
            return calibrationFactor;
        }

        /**
         *@listenData 'response of 'I' received from serial port'
         * return array of calibrationfactors 
         */
        public static String[] extractCalibrationNumerics(String calibrationFactor)
        {
            String lData = calibrationFactor;
            int length = lData.Length;
            String[] calibrationNumerics = new String[3];
            int calFactorXPosition = lData.IndexOf("Calibrationfactor_x");
            int calFactorYPosition = lData.IndexOf("Calibrationfactor_y");
            int calFactorZPosition = lData.IndexOf("Calibrationfactor_z");
            //substring is different to that of java, second parameter is not end of string, but the length of the string
            calibrationNumerics[0] = lData.Substring(calFactorXPosition + 20, (calFactorYPosition - 1) - (calFactorXPosition + 20));
            calibrationNumerics[1] = lData.Substring(calFactorYPosition + 20, (calFactorZPosition - 1) - (calFactorYPosition + 20));
            lData = lData.Substring(calFactorZPosition + 20);
            calibrationNumerics[2] = lData.Substring(0, lData.IndexOf(">"));
            return calibrationNumerics;
        }
    }
}
