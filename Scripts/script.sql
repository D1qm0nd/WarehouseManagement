

CREATE OR REPLACE FUNCTION "ON_RECEIPT_RESOURCES_UPDATE_FUNC"()
    RETURNS TRIGGER AS
$$
BEGIN
    INSERT INTO "WarehouseDbSchema"."ResourceBalances" ("Id", "ResourceId", "UnitOfMeasurementId", "Count", "Condition")
    VALUES (OLD."ResourceId", OLD."ResourceId", OLD."UnitOfMeasurementId", OLD."Count", 0);

END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER "UPDATE_BALANCE_AFTER_UPDATE" AFTER INSERT ON "WarehouseDbSchema"."ReceiptResources"
    FOR EACH ROW 
    EXECUTE FUNCTION "ON_RECEIPT_RESOURCES_UPDATE_FUNC"();

