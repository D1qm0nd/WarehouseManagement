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

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_CLIENT_SHIPPING_DOCUMENTS(client_id uuid)
    RETURNS TABLE
            (
                "Id"        uuid,
                "Number"    numeric(20),
                "Date"      timestamp with time zone,
                "ClientId"  uuid,
                "Status"    integer,
                "Condition" integer
            )
AS
$$
BEGIN
    RETURN QUERY SELECT SD."Id", SD."Number", SD."Date", SD."ClientId", SD."Status", SD."Condition"
                 FROM "WarehouseDbSchema"."ShippingDocument" AS SD
                 WHERE SD."ClientId" = client_id
                   AND SD."Condition" = 0;
END;
$$ LANGUAGE plpgsql;


SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION CHECK_ACTIVE_CLIENT_SHIPPING_DOCUMENTS()
    RETURNS TRIGGER
AS
$$
BEGIN
    IF (NEW."Condition" = 1) THEN
        IF EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_CLIENT_SHIPPING_DOCUMENTS(OLD."Id"))
        THEN
            RAISE EXCEPTION 'Denied';
        END IF;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE TRIGGER ON_ARCHIVE_CLIENT
    BEFORE UPDATE OF "Condition"
    ON "WarehouseDbSchema"."Clients"
    FOR EACH ROW
EXECUTE FUNCTION "WarehouseDbSchema".CHECK_ACTIVE_CLIENT_SHIPPING_DOCUMENTS();

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_RESOURCE_SHIPPING(resourceId uuid)
    RETURNS TABLE
            (
                "Id"                  uuid,
                "ResourceId"          uuid,
                "ShippingDocumentId"  uuid,
                "UnitOfMeasurementId" text,
                "Count"               numeric,
                "Condition"           integer,
                "State"               integer
            )
AS
$$
BEGIN
    RETURN QUERY SELECT *
                 FROM "WarehouseDbSchema"."ShippingResources" AS SR
                 WHERE SR."ResourceId" = resourceId
                   AND SR."Condition" = 0;
END
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_RESOURCE_RECEIPTS(resourceId uuid)
    RETURNS TABLE
            (
                "Id"                  uuid,
                "ReceiptDocumentId"   uuid,
                "ResourceId"          uuid,
                "UnitOfMeasurementId" text,
                "Count"               numeric,
                "Condition"           integer
            )
AS
$$
BEGIN
    RETURN QUERY SELECT *
                 FROM "WarehouseDbSchema"."ReceiptResources" AS RR
                 WHERE RR."ResourceId" = resourceId
                   AND RR."Condition" = 0;
END
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION CHECK_ACTIVE_RESOURCE_DOCUMENTS()
    RETURNS TRIGGER
AS
$$
BEGIN
    IF (OLD."Condition" = 0 AND NEW."Condition" = 1) THEN
        IF EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_RESOURCE_SHIPPING(OLD."Id")) THEN
            RAISE EXCEPTION 'Resource is active in shipping';
        ELSE
            IF EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_RESOURCE_RECEIPTS(OLD."Id")) THEN
                RAISE EXCEPTION 'Resource is active in receipts';
            END IF;
        END IF;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE TRIGGER ON_ARCHIVE_RESOURCE
    BEFORE UPDATE
    ON "WarehouseDbSchema"."Resources"
    FOR EACH ROW
EXECUTE FUNCTION "WarehouseDbSchema".CHECK_ACTIVE_RESOURCE_DOCUMENTS();


SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_RECEIPT_RESOURCES(unitId text)
    RETURNS TABLE
            (
                "Id"                  uuid,
                "ReceiptDocumentId"   uuid,
                "ResourceId"          uuid,
                "UnitOfMeasurementId" text,
                "Count"               numeric,
                "Condition"           integer
            )
AS
$$
BEGIN
    RETURN QUERY (SELECT *
                  FROM "WarehouseDbSchema"."ReceiptResources" AS RR
                  WHERE RR."UnitOfMeasurementId" = unitId AND RR."Condition" = 0);
END;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(unitId text)
    RETURNS TABLE
            (
                "Id"                  uuid,
                "ResourceId"          uuid,
                "ShippingDocumentId"  uuid,
                "UnitOfMeasurementId" text,
                "Count"               numeric,
                "Condition"           integer,
                "State"               integer
            )
AS
$$
BEGIN
    RETURN QUERY (SELECT *
                  FROM "WarehouseDbSchema"."ShippingResources" AS SR
                  WHERE SR."UnitOfMeasurementId" = unitId AND SR."Condition" = 0);
END;
$$ LANGUAGE plpgsql;


SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES(unitId text)
    RETURNS TABLE
            (
                "Id"                  uuid,
                "Name"                text,
                "UnitOfMeasurementId" text,
                "Condition"           integer
            )
AS
$$
BEGIN
    RETURN QUERY (SELECT *
                  FROM "WarehouseDbSchema"."Resources" AS R
                  WHERE R."UnitOfMeasurementId" = unitId AND R."Condition" = 0);
END;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE FUNCTION CHECK_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES()
    RETURNS TRIGGER
AS
$$
BEGIN
    IF (EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(OLD."Id"))) THEN
        RAISE EXCEPTION 'Unit of measurement using in shipping resources';
    ELSE
        IF (EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(OLD."Id"))) THEN
            RAISE EXCEPTION 'Unit of measurement using in receipts resources';
        ELSE
            IF (EXISTS(SELECT 1 FROM "WarehouseDbSchema".GET_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES(OLD."Id"))) THEN
                RAISE EXCEPTION 'Unit of measurement using in resources';
            END IF;
        END IF;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

SET search_path TO "WarehouseDbSchema";
CREATE OR REPLACE TRIGGER ON_ARCHIVE_UNIT_OF_MEASUREMENT
    BEFORE UPDATE
    ON "WarehouseDbSchema"."UnitsOfMeasurement"
    FOR EACH ROW
EXECUTE FUNCTION CHECK_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES();