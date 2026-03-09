CREATE TABLE public.outbox_messages (
    id uuid NOT NULL,
    aggregate_type character varying(200),
    aggregate_id uuid,
    type character varying(200) NOT NULL,
    payload text NOT NULL,
    occurred_on timestamp with time zone NOT NULL,
    processed_on timestamp with time zone,
    error text,
    CONSTRAINT pk_outbox_messages PRIMARY KEY (id)
);


CREATE TABLE public.permissions (
    id uuid NOT NULL,
    code character varying(150) NOT NULL,
    description text,
    category character varying(100),
    CONSTRAINT pk_permissions PRIMARY KEY (id)
);


CREATE TABLE tenants (
    id uuid NOT NULL,
    name character varying(200) NOT NULL,
    domain character varying(200),
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_tenants PRIMARY KEY (id)
);


CREATE TABLE public.audit_logs (
    id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    entity_name character varying(200) NOT NULL,
    entity_id uuid NOT NULL,
    action character varying(100) NOT NULL,
    old_values text,
    new_values text,
    performed_by uuid,
    performed_at timestamp with time zone NOT NULL,
    correlation_id character varying(100),
    CONSTRAINT pk_audit_logs PRIMARY KEY (id),
    CONSTRAINT fk_audit_logs_tenant FOREIGN KEY (tenant_id) REFERENCES tenants (id) ON DELETE RESTRICT
);


CREATE TABLE public.roles (
    id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    name character varying(150) NOT NULL,
    normalized_name character varying(150) NOT NULL,
    description text,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_roles PRIMARY KEY (id),
    CONSTRAINT fk_roles_tenant FOREIGN KEY (tenant_id) REFERENCES tenants (id) ON DELETE RESTRICT
);


CREATE TABLE public.users (
    id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    email character varying(256) NOT NULL,
    normalized_email character varying(256) NOT NULL,
    username character varying(100) NOT NULL,
    normalized_username character varying(100) NOT NULL,
    password_hash text NOT NULL,
    email_confirmed boolean NOT NULL DEFAULT FALSE,
    status integer NOT NULL,
    failed_login_attempts integer NOT NULL DEFAULT 0,
    lockout_end timestamp with time zone,
    last_login_at timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    created_by uuid,
    updated_at timestamp with time zone,
    updated_by uuid,
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT fk_users_tenant FOREIGN KEY (tenant_id) REFERENCES tenants (id) ON DELETE RESTRICT
);


CREATE TABLE public.role_permissions (
    role_id uuid NOT NULL,
    permission_id uuid NOT NULL,
    CONSTRAINT pk_role_permissions PRIMARY KEY (role_id, permission_id),
    CONSTRAINT fk_role_permissions_permission FOREIGN KEY (permission_id) REFERENCES public.permissions (id) ON DELETE CASCADE,
    CONSTRAINT fk_role_permissions_role FOREIGN KEY (role_id) REFERENCES public.roles (id) ON DELETE CASCADE
);


CREATE TABLE public.login_history (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    tenant_id uuid NOT NULL,
    login_time timestamp with time zone NOT NULL,
    ip_address character varying(100),
    device_info text,
    location character varying(200),
    is_successful boolean NOT NULL,
    CONSTRAINT pk_login_history PRIMARY KEY (id),
    CONSTRAINT fk_login_history_tenant FOREIGN KEY (tenant_id) REFERENCES tenants (id) ON DELETE RESTRICT,
    CONSTRAINT fk_login_history_user FOREIGN KEY (user_id) REFERENCES public.users (id) ON DELETE CASCADE
);


CREATE TABLE public.refresh_tokens (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    token_hash character varying(512) NOT NULL,
    expires_at timestamp with time zone NOT NULL,
    created_at timestamp with time zone NOT NULL,
    revoked_at timestamp with time zone,
    replaced_by_token_id uuid,
    created_by_ip character varying(100),
    CONSTRAINT pk_refresh_tokens PRIMARY KEY (id),
    CONSTRAINT fk_refresh_tokens_replaced_by FOREIGN KEY (replaced_by_token_id) REFERENCES public.refresh_tokens (id) ON DELETE SET NULL,
    CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES public.users (id) ON DELETE CASCADE
);


CREATE TABLE public.user_roles (
    user_id uuid NOT NULL,
    role_id uuid NOT NULL,
    CONSTRAINT pk_user_roles PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES public.roles (id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES public.users (id) ON DELETE CASCADE
);


CREATE INDEX ix_audit_logs_entity ON public.audit_logs (entity_name, entity_id);


CREATE INDEX ix_audit_logs_tenant_id ON public.audit_logs (tenant_id);


CREATE INDEX ix_login_history_login_time ON public.login_history (login_time);


CREATE INDEX "IX_login_history_tenant_id" ON public.login_history (tenant_id);


CREATE INDEX ix_login_history_user_id ON public.login_history (user_id);


CREATE INDEX ix_outbox_messages_processed_on ON public.outbox_messages (processed_on);


CREATE UNIQUE INDEX ux_permissions_code ON public.permissions (code);


CREATE INDEX ix_refresh_tokens_expires_at ON public.refresh_tokens (expires_at);


CREATE INDEX "IX_refresh_tokens_replaced_by_token_id" ON public.refresh_tokens (replaced_by_token_id);


CREATE INDEX ix_refresh_tokens_user_id ON public.refresh_tokens (user_id);


CREATE INDEX "IX_role_permissions_permission_id" ON public.role_permissions (permission_id);


CREATE INDEX ix_roles_tenant_id ON public.roles (tenant_id);


CREATE UNIQUE INDEX ux_roles_tenant_normalized_name ON public.roles (tenant_id, normalized_name);


CREATE UNIQUE INDEX ux_tenants_domain ON tenants (domain);


CREATE INDEX "IX_user_roles_role_id" ON public.user_roles (role_id);


CREATE INDEX ix_users_tenant_id ON public.users (tenant_id);


CREATE UNIQUE INDEX ux_users_tenant_normalized_email ON public.users (tenant_id, normalized_email);


CREATE UNIQUE INDEX ux_users_tenant_normalized_username ON public.users (tenant_id, normalized_username);

