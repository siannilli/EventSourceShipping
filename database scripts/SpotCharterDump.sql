--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5.5
-- Dumped by pg_dump version 9.5.5

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- Name: CommitDispatchedEvent(uuid, character varying, character varying); Type: FUNCTION; Schema: public; Owner: spot_user
--

CREATE FUNCTION "CommitDispatchedEvent"(_id uuid, _host character varying, _process character varying) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
UPDATE spot_events_dispatch
	SET dispatched = true, dispatch_datetime = now(), dispatch_host = _host, dispatch_process = _process
WHERE event_id = _id;

return _id;

END;
$$;


ALTER FUNCTION public."CommitDispatchedEvent"(_id uuid, _host character varying, _process character varying) OWNER TO spot_user;

--
-- Name: CreateSpotEventDispatcherRow(); Type: FUNCTION; Schema: public; Owner: spot_role
--

CREATE FUNCTION "CreateSpotEventDispatcherRow"() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO spot_events_dispatch(event_id)
	VALUES (NEW.id);

RETURN NEW;
END;
$$;


ALTER FUNCTION public."CreateSpotEventDispatcherRow"() OWNER TO spot_role;

--
-- Name: FUNCTION "CreateSpotEventDispatcherRow"(); Type: COMMENT; Schema: public; Owner: spot_role
--

COMMENT ON FUNCTION "CreateSpotEventDispatcherRow"() IS 'Creates an entry into spot_events_dispatch';


--
-- Name: GetEventToDispatch(character varying, character varying); Type: FUNCTION; Schema: public; Owner: spot_role
--

CREATE FUNCTION "GetEventToDispatch"(_host character varying, _process character varying, OUT id uuid, OUT event_name character varying, OUT source character varying, OUT date_time timestamp with time zone, OUT version integer, OUT payload json) RETURNS record
    LANGUAGE plpgsql
    AS $$

BEGIN
	
SELECT sed.event_id, se.event_name, se.date_time, se.version, se.payload, se.source INTO id, event_name, date_time, version, payload, source
FROM spot_events_dispatch sed, spot_events se
WHERE sed.event_id = se.id
	AND dispatched = false 
	AND (dispatch_lock IS NULL OR age(now(), dispatch_lock) > INTERVAL '30 seconds')
ORDER BY se.date_time
FETCH FIRST ROW ONLY;

if (id IS NOT null) THEN
	UPDATE spot_events_dispatch sed
		SET dispatch_host = _host, dispatch_process = _process, dispatch_lock = now()
	WHERE sed.event_id = id;			
END IF;

END;
$$;


ALTER FUNCTION public."GetEventToDispatch"(_host character varying, _process character varying, OUT id uuid, OUT event_name character varying, OUT source character varying, OUT date_time timestamp with time zone, OUT version integer, OUT payload json) OWNER TO spot_role;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: spot_events; Type: TABLE; Schema: public; Owner: spot_user
--

CREATE TABLE spot_events (
    id uuid NOT NULL,
    date_time timestamp with time zone NOT NULL,
    version integer NOT NULL,
    event_name character varying NOT NULL,
    aggregate_type character varying NOT NULL,
    source character varying,
    payload json NOT NULL,
    payload_type character varying NOT NULL,
    aggregate_id uuid NOT NULL
);


ALTER TABLE spot_events OWNER TO spot_user;

--
-- Name: spot_events_dispatch; Type: TABLE; Schema: public; Owner: spot_user
--

CREATE TABLE spot_events_dispatch (
    event_id uuid NOT NULL,
    dispatched boolean DEFAULT false NOT NULL,
    dispatch_lock timestamp with time zone DEFAULT now() NOT NULL,
    dispatch_host character varying,
    dispatch_process character varying,
    dispatch_datetime timestamp with time zone
);


ALTER TABLE spot_events_dispatch OWNER TO spot_user;

--
-- Name: spotEventsDispatchPK; Type: CONSTRAINT; Schema: public; Owner: spot_user
--

ALTER TABLE ONLY spot_events_dispatch
    ADD CONSTRAINT "spotEventsDispatchPK" PRIMARY KEY (event_id);


--
-- Name: spot_events_pkey; Type: CONSTRAINT; Schema: public; Owner: spot_user
--

ALTER TABLE ONLY spot_events
    ADD CONSTRAINT spot_events_pkey PRIMARY KEY (id);


--
-- Name: CreateEventToDispatch; Type: TRIGGER; Schema: public; Owner: spot_user
--

CREATE TRIGGER "CreateEventToDispatch" AFTER INSERT ON spot_events FOR EACH ROW EXECUTE PROCEDURE "CreateSpotEventDispatcherRow"();


--
-- Name: SpotEventIIdFK; Type: FK CONSTRAINT; Schema: public; Owner: spot_user
--

ALTER TABLE ONLY spot_events_dispatch
    ADD CONSTRAINT "SpotEventIIdFK" FOREIGN KEY (event_id) REFERENCES spot_events(id);


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

