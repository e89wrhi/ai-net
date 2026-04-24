const base_url = 'api';

export const api_paths = {
    user: {
        track_activity: (): string => `${base_url}/v1/users/track-activity`,
        reset_usage: (): string => `${base_url}/v1/users/reset-usage`,
        usage_summary: (): string => `${base_url}/v1/users/usage-summary`,
        activities: (): string => `${base_url}/v1/users/activities`,
        persona: (): string => `${base_url}/v1/user/persona`,
        analyze_usage: (): string => `${base_url}/v1/user/analyze-usage`,
    },
    translate: {
        translate: (): string => `${base_url}/v1/translate/translate`,
        stream_translate: (): string => `${base_url}/v1/translate/translate-stream`,
        detect: (): string => `${base_url}/v1/translate/detect`,
    },
    simple_md: {
        summarize: (): string => `${base_url}/v1/md/summarize`,
        stream_summarize: (): string => `${base_url}/v1/md/stream_summarize`,
        chat: (): string => `${base_url}/v1/md/chat`,
        stream_chat: (): string => `${base_url}/v1/md/stream_chat`,
    },
    payment: {
        record_charge: (): string => `${base_url}/v1/subscription/charge/record`,
        cancel_subscription: (): string => `${base_url}/v1/subscription/cancel`,
        get_invoices: (subscriptionId: string): string => `${base_url}/v1/subscription/${subscriptionId}/invoices`,
        get_subscription: (): string => `${base_url}/v1/subscription`,
        forecast: (): string => `${base_url}/v1/payment/forecast`,
        generate_invoice: (): string => `${base_url}/v1/subscription/invoice/generate`,
        analyze_invoice: (): string => `${base_url}/v1/payment/analyze-invoice`,
        create_subscription: (): string => `${base_url}/v1/subscription/create`,
    },
    image_gen: {
        generate: (): string => `${base_url}/v1/imagegen/generate`,
        regenerate: (): string => `${base_url}/v1/imagegen/regenerate`,
    },
    identity: {
        register: (): string => `${base_url}/v1/identity/register-user`,
    },
    image_edit: {
        remove_background: (): string => `${base_url}/v1/imageedit/remove-background`,
        enhance: (): string => `${base_url}/v1/imageedit/enhance`,
    },
    chat: {
        update: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
        stream_response: (): string => `${base_url}/v1/chat/stream-response`,
        start: (): string => `${base_url}/v1/chat`,
        send_message: (): string => `${base_url}/v1/chat/send-message`,
        history: (): string => `${base_url}/v1/chat/history`,
        get_by_id: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
        generate_response: (): string => `${base_url}/v1/chat/generate-response`,
        delete: (sessionId: string): string => `${base_url}/v1/chat/${sessionId}`,
    },
    code_debug: {
        stream_analyze: (): string => `${base_url}/v1/codedebug/analyze-stream`,
        fix: (): string => `${base_url}/v1/codedebug/fix`,
        analyze: (): string => `${base_url}/v1/codedebug/analyze`,
    },
    code_gen: {
        stream_generate: (): string => `${base_url}/v1/codegen/generate-stream`,
        regenerate: (): string => `${base_url}/v1/codegen/regenerate`,
        generate: (): string => `${base_url}/v1/codegen/generate`,
    },
};
