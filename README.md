# the-access-app

## The Challenge:
Create a program to manage hotel room availability and reservations. The application should read from files containing hotel data and reservation data, then allow a user to check room availability for a specified hotel, date range, and room type. 

About the app:

1. **How to run the application**
    - identify the location of the solution
    - open PowerShell
    - change directory to the application entry point
        ```console
        cd C:\username\Projects\the-access-app\src\console
        ```
    - build to check for errors
        ```console
        dotnet build
        ```
    - run the application providing hotels and bookings file locations as arguments (you can find some samples below)
        ```console
        dotnet run -- --bookings C:\username\Projects\the-access-app\files\bookings.json --hotels C:\username\Projects\the-access-app\files\hotels.json
        ```
    - run a command (check the list of available commands below)
    - the application will exit when a blank line is entered

2. **Json files format**
    - hotels.json
        ```json
        [{"id":"H1","name":"Hotel California","roomTypes":[{"code":"SGL","size":1,"description":"Single Room","amenities":["WiFi","TV"],"features":["Non-smoking"]},{"code":"DBL","size":2,"description":"Double Room","amenities":["WiFi","TV","Minibar"],"features":["Non-smoking","Sea View"]}],"rooms":[{"roomType":"SGL","roomId":"101"},{"roomType":"SGL","roomId":"102"},{"roomType":"DBL","roomId":"201"},{"roomType":"DBL","roomId":"202"}]}]
        ```
    - bookings.json
        ```json
        [{"hotelId":"H1","arrival":"20240901","departure":"20240903","roomType":"DBL","roomRate":"Prepaid"},{"hotelId":"H1","arrival":"20240902","departure":"20240905","roomType":"SGL","roomRate":"Standard"},{"hotelId":"H1","arrival":"20240904","departure":"20240905","roomType":"SGL","roomRate":"Standard"}]
        ```

3. **Available Commands**
    1. Availability Command
        - Console Input Examples
            ```console
            Availability(H1, 20240901, SGL)
            ```
            ```console
            Availability(H1, 20240901-20240903, DBL)
            ```
        - Expected Output: the program should give the availability as a number for a room type that date range. Note: hotels sometimes accept overbookings so the value can be negative to indicate this

    2. RoomTypes Command
        - Console Input Examples
            ```console
            RoomTypes(H1, 20240904, 3)
            ```
            ```console
            RoomTypes(H1, 20240905-20240907, 5)
            ```
        - Expected Output: The program should return a hotel id, and a comma separated list of room type codes needed to allocate number of people specified in the specified time. Minimise the number of rooms to accommodate the required number of people. Avoid partially filling rooms. If a room is partially filled, the room should be marked with a "!”. Show an error message if allocation is not possible.
        - Console Output Examples:
            ```console
            H1: DBL, DBL!
            ```
            ```console
            H1: DBL, DBL, SGL
            ```

## Additional Notes
- the application is using .net8
- check the json file content to be in the correct format
- executing an invalid command will return an error message
- a booking is considered valid only if `arrival date > departure date`
