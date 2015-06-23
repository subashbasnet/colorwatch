using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static String dataOutWrite(String dataOutputResponse)
        {
            int dataOutResponseIndex = dataOutputResponse.IndexOf("Data_Output<");
            if (dataOutResponseIndex > -1)
            {
                if (dataOutputResponse.Substring(dataOutResponseIndex + 12,
                1).Equals("1"))
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            else
            {
                return "unknown";
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

        /**
         * @return array of RLab data i.e. 'a' and 'b' value from manual start i.e. 'S' response
         * **/
        public static string[] RLabForGraphOnly(string rLab)
        {
            string[] abValue = new string[4];
            int manualStartAPosition = rLab.IndexOf(">a<");
            int manualStartBPosition = rLab.IndexOf(">b<");
            int manualStartHPosition = rLab.IndexOf(">h<");
            if (manualStartAPosition > -1 && manualStartBPosition > -1 && manualStartHPosition > -1)
            {
                abValue[0] = rLab.Substring(manualStartAPosition + 3, (manualStartBPosition) - (manualStartAPosition + 3));
                abValue[1] = rLab.Substring(manualStartBPosition + 3, (manualStartHPosition) - (manualStartBPosition + 3));
            }
            return abValue;
        }

        /**
         * @return array of RLab data i.e. 'a' and 'b' value from manual start i.e. 'S' response
         * **/
        public static string[] RLabManualStart(string rLab)
        {
            string[] abValue = new string[4];
            int manualStartAPosition = rLab.IndexOf(">a<");
            int manualStartBPosition = rLab.IndexOf(">b<");
            int manualStartHPosition = rLab.IndexOf(">h<");
            int indexOfDigitalInputLightness = rLab.IndexOf("digital_input_lightness");
            int indexOfDigitalOutputMeasurementColour = rLab.IndexOf("digital_output_measurement_colour");
            int firstIndexOfDigitalOutputError = rLab.IndexOf("digital_output_error");
            if (manualStartAPosition > -1 && manualStartBPosition > -1 && manualStartHPosition > -1
                && indexOfDigitalInputLightness > -1 && indexOfDigitalOutputMeasurementColour > -1
                && firstIndexOfDigitalOutputError > -1)
            {
                abValue[0] = rLab.Substring(manualStartAPosition + 3, (manualStartBPosition) - (manualStartAPosition + 3));
                abValue[1] = rLab.Substring(manualStartBPosition + 3, (manualStartHPosition) - (manualStartBPosition + 3));
                abValue[2] = rLab.Substring(indexOfDigitalInputLightness + 24,
                    (indexOfDigitalOutputMeasurementColour - 1) - (indexOfDigitalInputLightness + 24));//DI1
                abValue[3] = rLab.Substring(indexOfDigitalOutputMeasurementColour + 34,
                    (firstIndexOfDigitalOutputError - 1) - (indexOfDigitalOutputMeasurementColour + 34));//DO1
            }
            return abValue;
        }

        /**
         * @return 'h' values from manual start i.e. 'S' response
         * **/
        public static double hValue(string manualStartResponseData)
        {
            int manualStartHPosition = manualStartResponseData.IndexOf("h<");
            int manualStartrawLPosition = manualStartResponseData.IndexOf("rawL<");
            double hValue = double.Parse(manualStartResponseData.Substring(manualStartHPosition + 2,
                (manualStartrawLPosition - 2) - (manualStartHPosition + 2)));
            return hValue;
        }

        /**
         *Extract Border_Pink, Border_Green, Border_Yellow, Border_Clear values
         * 
         */
        //this must be wrong as last index for ">" taken previously, should be changed now
        public static double[] extractBorderValues(String listenReponse)
        {
            int length = listenReponse.Length;
            double[] hValues = new double[4];
            int borderPink = listenReponse.IndexOf("Border_Pink");
            int borderGreen = listenReponse.IndexOf("Border_Green");
            int borderYellow = listenReponse.IndexOf("Border_Yellow");
            int borderClear = listenReponse.IndexOf("Border_Clear");
            hValues[0] = double.Parse(listenReponse.Substring(borderPink + 12, (borderGreen - 1) - (borderPink + 12)));
            hValues[1] = double.Parse(listenReponse.Substring(borderGreen + 13, (borderYellow - 1) - (borderGreen + 13)));
            hValues[2] = double.Parse(listenReponse.Substring(borderYellow + 14, (borderClear - 1) - (borderYellow + 14)));
            hValues[3] = double.Parse(listenReponse.Substring(borderClear + 13, listenReponse.LastIndexOf(">") - (borderClear + 13)));
            return hValues;
        }

        /**
         *Extract Probetimer, Conducttimer, pwm_cycle, 
         *conductivity_abweichung, conductivity_data_average values
         */
        public static double[] extractTimerValues(String listenReponse)
        {
            int length = listenReponse.Length;
            double[] timerValues = new double[6];
            int indexOfReferenceTimer = listenReponse.IndexOf("Referencetimer");
            int indexOfProbeTimer = listenReponse.IndexOf("Probetimer");
            int indexOfConductTimer = listenReponse.IndexOf("Conducttimer");
            int indexOfPwmCycle = listenReponse.IndexOf("pwm_cycle");
            int indexOfConductivityAbweichung = listenReponse.IndexOf("conductivity_abweichung");
            int indexOfConductivityDataAverage = listenReponse.IndexOf("conductivity_data_average");
            timerValues[0] = double.Parse(listenReponse.Substring(indexOfReferenceTimer + 15, (indexOfProbeTimer - 1) - (indexOfReferenceTimer + 15)));
            timerValues[1] = double.Parse(listenReponse.Substring(indexOfProbeTimer + 11, (indexOfConductTimer - 1) - (indexOfProbeTimer + 11)));
            timerValues[2] = double.Parse(listenReponse.Substring(indexOfConductTimer + 13, (indexOfPwmCycle - 1) - (indexOfConductTimer + 13)));
            timerValues[3] = double.Parse(listenReponse.Substring(indexOfPwmCycle + 10, (indexOfConductivityAbweichung - 1) - (indexOfPwmCycle + 10)));
            timerValues[4] = double.Parse(listenReponse.Substring(indexOfConductivityAbweichung + 24, (indexOfConductivityDataAverage - 1) - (indexOfConductivityAbweichung + 24)));
            timerValues[5] = double.Parse(listenReponse.Substring(indexOfConductivityDataAverage + 26, listenReponse.LastIndexOf(">") - (indexOfConductivityDataAverage + 26)));
            return timerValues;
        }

        /**
         * Extract sauber condition of manual start and measuring_conductivity
         * 
         * **/
        public static List<string> extractSauberCondition(String responseData)
        {
            List<string> continuousResponse = new List<string>();
            int indexOfDigitalInputLightness = responseData.IndexOf("digital_input_lightness");
            int indexOfDigitalOutputMeasurementColour = responseData.IndexOf("digital_output_measurement_colour");
            int firstIndexOfDigitalOutputError = responseData.IndexOf("digital_output_error");
            int indexOfDigitalInputConductivity = responseData.IndexOf("digital_input_conductivity");
            int indexOfDigitalOutputMeasurementConductivity = responseData.IndexOf("digital_output_measurement_conductivity");
            int lastIndexOfDigitalOutputError = responseData.LastIndexOf("digital_output_error");
            if (indexOfDigitalInputLightness > -1 && indexOfDigitalOutputMeasurementColour > -1
                && firstIndexOfDigitalOutputError > -1 && indexOfDigitalInputConductivity > -1
                && indexOfDigitalOutputMeasurementConductivity > -1
                && lastIndexOfDigitalOutputError > -1
                && firstIndexOfDigitalOutputError != lastIndexOfDigitalOutputError)//continuous messung 
            {
                continuousResponse.Add("All");
                continuousResponse.Add(responseData.Substring(indexOfDigitalInputLightness + 24,
                    (indexOfDigitalOutputMeasurementColour - 1) - (indexOfDigitalInputLightness + 24)));//DI1
                continuousResponse.Add(responseData.Substring(indexOfDigitalOutputMeasurementColour + 34,
                    (firstIndexOfDigitalOutputError - 1) - (indexOfDigitalOutputMeasurementColour + 34)));//DO1
                continuousResponse.Add(responseData.Substring(indexOfDigitalInputConductivity + 27,
                    (indexOfDigitalOutputMeasurementConductivity - 1) - (indexOfDigitalInputConductivity + 27)));//DI2
                continuousResponse.Add(responseData.Substring(indexOfDigitalOutputMeasurementConductivity + 40,
                    (lastIndexOfDigitalOutputError - 1) - (indexOfDigitalOutputMeasurementConductivity + 40)));//DO2
                return continuousResponse;
            }
            else if (indexOfDigitalInputLightness > -1 && indexOfDigitalOutputMeasurementColour > -1
                && firstIndexOfDigitalOutputError > -1)//manual start
            {
                continuousResponse.Add("S");
                continuousResponse.Add(responseData.Substring(indexOfDigitalInputLightness + 24,
                    (indexOfDigitalOutputMeasurementColour - 1) - (indexOfDigitalInputLightness + 24)));//DI1
                continuousResponse.Add(responseData.Substring(indexOfDigitalOutputMeasurementColour + 34,
                    (firstIndexOfDigitalOutputError - 1) - (indexOfDigitalOutputMeasurementColour + 34)));//DO1
                return continuousResponse;
            }
            else if (indexOfDigitalInputConductivity > -1 && indexOfDigitalOutputMeasurementConductivity > -1
              && lastIndexOfDigitalOutputError > -1)//measuring conductivity
            {
                continuousResponse.Add("G");
                continuousResponse.Add(responseData.Substring(indexOfDigitalInputConductivity + 27,
                    (indexOfDigitalOutputMeasurementConductivity - 1) - (indexOfDigitalInputConductivity + 27)));//DI2
                continuousResponse.Add(responseData.Substring(indexOfDigitalOutputMeasurementConductivity + 40,
                    (lastIndexOfDigitalOutputError - 1) - (indexOfDigitalOutputMeasurementConductivity + 40)));//DO2
                return continuousResponse;
            }
            else
            {
                return null;
            }
        }

        /**
         * getting the index of nth occurrence of char in a string
         * **/
        public static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /**
         * Only the number entry is allowed but be more than one '.' is also allowed and backspace
         * **/
        public static void restrictCharsFunction(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back) || e.KeyChar == '.'))
                e.Handled = true;
        }

        public static String trimForMoreThanOneDotInInputNumber(String number)
        {
            int count = number.Trim().Count(x => x == '.');
            if (count > 1)
            {
                //trim the text after second '.'
                int indexOfSecondOccurrence = GetNthIndex(number, '.', 2);
                return number.Substring(0, indexOfSecondOccurrence);
            }
            return number.Trim();
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