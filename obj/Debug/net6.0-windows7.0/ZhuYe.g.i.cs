﻿#pragma checksum "..\..\..\ZhuYe.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "05A2480798E2D11E09577F6C718A28AE25005640"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MCL_Dev;
using Panuon.UI.Silver;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MCL_Dev {
    
    
    /// <summary>
    /// ZhuYe
    /// </summary>
    public partial class ZhuYe : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button start;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContentControl login;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock gameVersion;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox versionCombo;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button start_Copy;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox javaCombo;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock javaBanBen;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ZuiDaRAM;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox maxMem;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\ZhuYe.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button start_Copy1;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MCL-Dev;component/zhuye.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ZhuYe.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.start = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\ZhuYe.xaml"
            this.start.Click += new System.Windows.RoutedEventHandler(this.start_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.login = ((System.Windows.Controls.ContentControl)(target));
            return;
            case 3:
            this.gameVersion = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.versionCombo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.start_Copy = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\ZhuYe.xaml"
            this.start_Copy.Click += new System.Windows.RoutedEventHandler(this.start_Copy_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.javaCombo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.javaBanBen = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.ZuiDaRAM = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.maxMem = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.start_Copy1 = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\ZhuYe.xaml"
            this.start_Copy1.Click += new System.Windows.RoutedEventHandler(this.start_Copy1_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

