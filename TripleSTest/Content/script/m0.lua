import ('TripleS', 'TripleS.Scripting')

--Example level script.
--Use glob to get functions from your global script.
--You must have Init and Update as functions.

local glob = require("global")

function Init()
	glob.Foo()
end

function Update()
end