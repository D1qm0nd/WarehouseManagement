SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_RESOURCES()
    RETURNS TABLE
            (
                Id   uuid,
                Name text
            )
AS
$$
BEGIN
    RETURN QUERY SELECT "Id", "Name" FROM "WarehouseDbSchema"."Resources";
end;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_RECEIPT_RESOURCES()
    RETURNS TABLE
            (
                Id    uuid,
                Name  text,
                Count numeric
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT RD.Id, RD.Name, SUM(RR."Count")
        FROM "WarehouseDbSchema".GET_RESOURCES() AS RD
                 LEFT OUTER JOIN "WarehouseDbSchema"."ReceiptResources" as RR
                                 ON RD.Id = RR."ResourceId"
        WHERE RR."Condition" = 0
        GROUP BY RD.Id, RD.Name;
        
end;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_SHIPPED_RESOURCES()
    RETURNS TABLE
            (
                Id    uuid,
                Count numeric
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT SR."ResourceId", SUM(SR."Count")
        FROM "WarehouseDbSchema"."ShippingResources" AS SR
                 INNER JOIN "WarehouseDbSchema"."ShippingDocument" AS SD
                            ON SR."ShippingDocumentId" = SD."Id"
        WHERE "Status" = 2
        GROUP BY SR."ResourceId";
end;
$$ LANGUAGE plpgsql;


SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_RESOURCES_BALANCES()
    RETURNS TABLE
            (
                Id    uuid,
                Name  text,
                Count numeric
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT RR.Id,
               RR.Name,
               COALESCE(CASE
                            WHEN SR.Count IS NOT NULL
                                THEN RR.Count - SR.Count
                            ELSE RR.Count END, 0) AS Count
        FROM "WarehouseDbSchema".GET_SHIPPED_RESOURCES() AS SR
                 RIGHT OUTER JOIN "WarehouseDbSchema".GET_RECEIPT_RESOURCES() AS RR
                                  ON RR.Id = SR.Id;
end;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE VIEW "WarehouseDbSchema"."CurrentResourceBalances" AS
SELECT Id as "Id", Name as "Name", Count as "Count"
FROM GET_RESOURCES_BALANCES();


