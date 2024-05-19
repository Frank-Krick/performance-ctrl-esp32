using nanoFramework.Hosting;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Performance.Mixer.Controller.Esp32.Services;

internal class GpioWatcher
{
    public static void SetupGpioWatcher()
    {
        var gpioController = new GpioController();
        var pin = gpioController.OpenPin(5, PinMode.InputPullUp);
        pin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
        gpioController.RegisterCallbackForPinValueChangedEvent(5, PinEventTypes.Falling, (_, _) => {
            Debug.WriteLine("Falling edge detected");
        });
    }
}
