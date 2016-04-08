﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SiriusMicrotech.core.UI;
using System.Threading;

namespace Cad2D.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Page_Hsc_X : UserControl
    {
        private HscXHelper hscXHelper;
        public Page_Hsc_X()
        {
            InitializeComponent();
            load();

        }

        private void load()
        {/*
            hscXHelper= new HscXHelper();
            if (CanvasCad2D.lsConnection.Connected)
            {
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.testModeAXX.dataType,
                    hscXHelper.testModeAXX.valueAddress, ref hscXHelper.testModeAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.curPosMMAXX.dataType,
                    hscXHelper.curPosMMAXX.valueAddress, ref hscXHelper.curPosMMAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.spdReqAXX.dataType,
                    hscXHelper.spdReqAXX.valueAddress, ref hscXHelper.spdReqAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.accAXX.dataType,
                    hscXHelper.accAXX.valueAddress, ref hscXHelper.accAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.decModeAXX.dataType,
                    hscXHelper.decModeAXX.valueAddress, ref hscXHelper.decModeAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.manMaxTarPosAXX.dataType,
                    hscXHelper.manMaxTarPosAXX.valueAddress, ref hscXHelper.manMaxTarPosAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.manMinTarPosAXX.dataType,
                    hscXHelper.manMinTarPosAXX.valueAddress, ref hscXHelper.manMinTarPosAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.bwdKeshAXX.dataType,
                    hscXHelper.bwdKeshAXX.valueAddress, ref hscXHelper.bwdKeshAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.fwdKeshAXX.dataType,
                  hscXHelper.fwdKeshAXX.valueAddress, ref hscXHelper.fwdKeshAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.bwdMaxDispAXX.dataType,
                    hscXHelper.bwdMaxDispAXX.valueAddress, ref hscXHelper.bwdMaxDispAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.fwdMaxDispAXX.dataType,
                    hscXHelper.fwdMaxDispAXX.valueAddress, ref hscXHelper.fwdMaxDispAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.jogCalcAutoAXX.dataType,
                    hscXHelper.jogCalcAutoAXX.valueAddress, ref hscXHelper.jogCalcAutoAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.decAutoAXX.dataType,
                    hscXHelper.decAutoAXX.valueAddress, ref hscXHelper.decAutoAXX.readingPacket);
                CanvasCad2D.lsConnection.readFromPlc(hscXHelper.decStopAXX.dataType,
                    hscXHelper.decStopAXX.valueAddress, ref hscXHelper.decStopAXX.readingPacket);
            }*/
        }

        private void textBoxSpdReq_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {

            }
        }

        private void textBoxAcc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDec_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxManMax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxManMin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_12_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_13_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_14_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_15_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_18_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_19_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_20_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_21_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_22_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_23_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_24_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_25_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_26_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_27_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_28_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_29_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_30_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_31_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_32_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_33_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_34_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_35_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_36_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_37_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_38_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_39_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxjogArray_40_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecStop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_12_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_13_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_14_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_15_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_18_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_19_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_20_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_21_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_22_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_23_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_24_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_25_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_26_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_27_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_28_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_29_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_30_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_31_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_32_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_33_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_34_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_35_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_36_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_37_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_38_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_39_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void textBoxDecArray_40_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }
    }
}
