using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWatch
{
    static class BaseFunctions
    {


        /*
         * Get all serial ports list
         */
        public static Array GetAllPorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }
        /**
         *@listenData 'response of 'I' received from serial port'
         * return array of calibrationfactors 
         */
        public static String[] extractCalibrationFactor(String listenData)
        {
            String lData = listenData;
            int length = lData.Length;
            String[] calibrationFactor = new String[3];
            int calFactorXPosition = lData.IndexOf("Calibrationfactor_x");
            int calFactorYPosition = lData.IndexOf("Calibrationfactor_y");
            int calFactorZPosition = lData.IndexOf("Calibrationfactor_z");
            //substring is different to that of java, second parameter is not end of string, but the length of the string
            calibrationFactor[0] = lData.Substring(calFactorXPosition + 20, (calFactorYPosition - 1) - (calFactorXPosition + 20));
            calibrationFactor[1] = lData.Substring(calFactorYPosition + 20, (calFactorZPosition - 1) - (calFactorYPosition + 20));
            lData = lData.Substring(calFactorZPosition + 20);
            calibrationFactor[2] = lData.Substring(0, lData.IndexOf(">"));
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
            //substring is different to that of java,first parameter position of required char/string, 
            //second parameter is not end of string, but the length of the string 
            //i.e. the length of the output string i.e. length of our final result string
            calibrationNumerics[0] = lData.Substring(calFactorXPosition + 20, (calFactorYPosition - 1) - (calFactorXPosition + 20));
            calibrationNumerics[1] = lData.Substring(calFactorYPosition + 20, (calFactorZPosition - 1) - (calFactorYPosition + 20));
            lData = lData.Substring(calFactorZPosition + 20);
            calibrationNumerics[2] = lData.Substring(0, lData.IndexOf(">"));
            return calibrationNumerics;
        }

        /**
         * @return boolean, true for W = 1 
         * and false for W=0.
         * **/
        public static Boolean dataOutWrite(String dataOutputResponse)
        {
            if (dataOutputResponse.Substring(dataOutputResponse.IndexOf("<") + 1,
                1).Equals("0"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /**
        * @return boolean array
        * Digital Ausgang 1 -->  rot(false)/grun(true)
        * Digital Ausgang 2 --> rot(false)/grun(true)
        * Fehler 2 --> rot(false)/grun(true)
        * **/
        public static Boolean[] outPutTest(String outPutTest)
        {
            int length = outPutTest.Length;
            Boolean[] outPutTestCondition = new Boolean[3];
            int digitalOutputMeasurementOnePosition = outPutTest.IndexOf("digital_output_measurement_one");
            int digitalOutputMeasurementTwoPosition = outPutTest.IndexOf("digital_output_measurement_two");
            int digitalOutputErrorPosition = outPutTest.IndexOf("digital_output_error");
            //substring is different to that of java,first parameter position of required char/string, 
            //second parameter is not end of string, but the length of the string 
            //i.e. the length of the output string i.e. length of our final result string
            if (outPutTest.Substring(digitalOutputMeasurementOnePosition + 31, 1).Equals("0"))
            {
                outPutTestCondition[0] = false;
            }
            else
            {
                outPutTestCondition[0] = true;
            }
            if (outPutTest.Substring(digitalOutputMeasurementTwoPosition + 31, 1).Equals("0"))
            {
                outPutTestCondition[1] = false;
            }
            else
            {
                outPutTestCondition[1] = true;
            }
            if (outPutTest.Substring(digitalOutputErrorPosition + 21, 1).Equals("0"))
            {
                outPutTestCondition[2] = false;
            }
            else
            {
                outPutTestCondition[2] = true;
            }
            return outPutTestCondition;
        }

        /**
        * @return string array
        *Wir haben o/p from Manual Start-->’S’ .
        *We look into ‘digital_output_measurement_one<X>’-->D01 and 
        *‘digital_output_measurement_two<X>’--> D02
        *Sauber-->D01 High und D02 High, Pink farbe ziegen
        *Not Clean--> D01 Low und D02 High, Grun farbe ziegen
        *D01 High und D02 Low, Gelb farbe ziegen
        *Undefined -->D01 Low und D02 Low
        * **/
        public static string[] manualStart(string manualStart)
        {
            string[] colors = new string[2];
            int digitalInputStartStopPosition = manualStart.IndexOf("digital_input_start_stop");
            int digitalOutputMeasurementOnePosition = manualStart.IndexOf("digital_output_measurement_one");
            int digitalOutputMeasurementTwoPosition = manualStart.IndexOf("digital_output_measurement_two");
            //substring is different to that of java, second parameter is not end of string, but the length of the string 
            //i.e. the length of the output string i.e. length of our final result string
            if (manualStart.Substring(digitalInputStartStopPosition + 25, 1).Equals("0"))
            {
                colors[0] = "ON";
            }
            else
            {
                colors[0] = "OFF";
            }
            if (manualStart.Substring(digitalOutputMeasurementOnePosition + 31, 1).Equals("0") &&
                manualStart.Substring(digitalOutputMeasurementTwoPosition + 31, 1).Equals("0"))
            {
                colors[1] = "undefiniert";
            }
            else if (manualStart.Substring(digitalOutputMeasurementOnePosition + 31, 1).Equals("1") &&
                manualStart.Substring(digitalOutputMeasurementTwoPosition + 31, 1).Equals("0"))
            {
                colors[1] = "gelb";
            }
            else if (manualStart.Substring(digitalOutputMeasurementOnePosition + 31, 1).Equals("0") &&
               manualStart.Substring(digitalOutputMeasurementTwoPosition + 31, 1).Equals("1"))
            {
                colors[1] = "grun";
            }
            else
            {
                colors[1] = "pink";

            }
            return colors;
        }

        public static string[] RLab(string rLab)
        {
            string[] abValue = new string[2];
            int manualStartAPosition = rLab.IndexOf("a<");
            int manualStartBPosition = rLab.IndexOf("b<");
            int manualStartHPosition = rLab.IndexOf("h<");
            abValue[0] = rLab.Substring(manualStartAPosition + 2, (manualStartBPosition - 2) - (manualStartAPosition + 2));
            abValue[1] = rLab.Substring(manualStartBPosition + 2, (manualStartHPosition - 2) - (manualStartBPosition + 2));
            return abValue;
        }
    }
}









//ctrl+K+D for format code
//ctrl+C+V to duplicate line
//ctrl+; to find file directly
//shift+f12 to go to function definition
//press ctrl+k then ctrl+c for comment
//press ctrl+k then ctrl+u for uncomment
//I'm using 'visualbasic6' default system for shortcuts, other types could be used