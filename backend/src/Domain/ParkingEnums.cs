namespace ParkingApp.Api.Domain;

public enum UserRole
{
    Administrator,
    Operator
}

public enum UserStatus
{
    Active,
    Inactive
}

public enum SectorStatus
{
    Active,
    Inactive
}

public enum VehicleCategory
{
    Car,
    Motorcycle,
    Both
}

public enum VehicleType
{
    Car,
    Motorcycle
}

public enum SpotStatus
{
    Free,
    Occupied,
    Blocked,
    Maintenance
}

public enum TariffStatus
{
    Active,
    Inactive
}

public enum StayStatus
{
    Active,
    Completed,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Pix,
    DebitCard,
    CreditCard,
    Other
}

public enum CashStatus
{
    Open,
    Closed
}

public enum CashMovementType
{
    ExitPayment,
    Bleed,
    Supply,
    Adjustment
}
