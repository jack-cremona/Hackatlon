# Toll Gate Simulation Program

This program simulates the operation of toll gates, processing vehicles as they pass through. It calculates toll fees, logs vehicle entries and exits, and tracks the time taken for each vehicle to clear the toll gate. The simulation is multithreaded, allowing multiple toll gates to operate simultaneously.

## Features

- **Multithreaded Simulation**: Each toll gate operates in its own thread, allowing concurrent processing of vehicles.
- **Dynamic Configuration**: The number of toll gates, maximum number of cars, and maximum distance can be configured at runtime.
- **Toll Fee Calculation**: Calculates toll fees based on the type of vehicle and distance traveled.
- **Thread-Safe Logging**: Logs all events (vehicle entry, exit, and toll fee details) to a file in a thread-safe manner.
- **Vehicle Processing**: Simulates different vehicle types and payment methods with varying processing times.

## How It Works

1. **Initialization**: The program prompts the user to input:
   - The number of toll gates.
   - The maximum number of cars allowed to pass.
   - The maximum distance that can be traveled.

2. **Simulation**:
   - Each toll gate is represented by a thread.
   - Vehicles are randomly assigned a type (e.g., car, heavy vehicle) and a payment method (e.g., cash, Telepass).
   - The toll fee is calculated based on the vehicle type and distance traveled.
   - The program logs the entry, toll fee details, and exit of each vehicle.

3. **Logging**:
   - All events are logged to a file named `FileDiLog.txt` in the program's directory.
   - Logging is thread-safe to ensure data consistency.

## Usage

1. Compile and run the program.
2. Follow the prompts to configure the simulation:
   - Enter the number of toll gates.
   - Enter the maximum number of cars allowed.
   - Enter the maximum distance that can be traveled.
3. The simulation will start, and the program will display logs in the console and write them to `FileDiLog.txt`.

## Code Overview

### Main Components

- **`Main` Method**: Entry point of the program. Initializes the simulation and starts toll gate threads.
- **`TollGate` Method**: Simulates the operation of a single toll gate, including vehicle processing and logging.
- **`CalcoloPedaggio` Method**: Calculates the toll fee based on vehicle type and distance.
- **`ReturnTempo` Method**: Returns the processing time for a vehicle based on its type.
- **`Writer` Method**: Logs events to a file in a thread-safe manner.

### Constants

- **Waiting Times**:
  - `Telepass`: 3000 ms
  - `Cash`: 5000 ms
  - `HevyVehicle`: 8000 ms
- **Base Toll Fee**:
  - `BasePrice`: €0.50

## Example Output

How many toll gates do you want to add? 2\
How many cars do you want to let pass? 5\
What is the maximum distance that can be traveled? 100\
TIME 10:15:30.12 AM --> vehicle entered toll gate 0\
TOLL GATE 0 TIME 10:15:33.12 AM --> vehicle type: 1 km traveled 50 price paid 3.00 euros\
TOLL GATE 0 TIME 10:15:33.12 AM --> the vehicle has cleared the toll gate. Its time was 3000 milliseconds\

## Requirements

- **C# Version**: 7.3
- **.NET Framework**: 4.7.2

## Log File

The program generates a log file named `FileDiLog.txt` in the program's directory. This file contains detailed logs of all events during the simulation.

## Notes

- The program uses a mutex to ensure thread-safe operations when logging and updating shared variables.
- The toll fee calculation and waiting times are parameterized for easy modification.

## Future Improvements

- Allow configuration of waiting times and base toll fee via user input or a configuration file.
- Add support for additional vehicle types and payment methods.
- Enhance the logging format for better readability.

## License

This program is provided as-is for educational purposes. Feel free to modify and use it as needed.

