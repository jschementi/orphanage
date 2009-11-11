def test(app):
    return app.test("Import Python file from XAP", "2", "1 + 1")

def test_import(app):
    import bar.baz
    return bar.baz.test(app)
