Add-Type -AssemblyName System.Drawing
$img = [System.Drawing.Bitmap]::FromFile("CopyAlert_Logo.png")
$minX = $img.Width; $minY = $img.Height; $maxX = 0; $maxY = 0;
for($y=0; $y -lt $img.Height; $y++) {
    for($x=0; $x -lt $img.Width; $x++) {
        $px = $img.GetPixel($x, $y)
        if ($px.A -gt 0) {
            if ($x -lt $minX) { $minX = $x }
            if ($x -gt $maxX) { $maxX = $x }
            if ($y -lt $minY) { $minY = $y }
            if ($y -gt $maxY) { $maxY = $y }
        }
    }
}
if ($minX -le $maxX -and $minY -le $maxY) {
    $rect = New-Object System.Drawing.Rectangle($minX, $minY, ($maxX-$minX+1), ($maxY-$minY+1))
    $cropped = $img.Clone($rect, $img.PixelFormat)
    $img.Dispose()
    $cropped.Save("CopyAlert_Logo_cropped.png", [System.Drawing.Imaging.ImageFormat]::Png)
    $cropped.Dispose()
    Remove-Item "CopyAlert_Logo.png" -Force
    Rename-Item "CopyAlert_Logo_cropped.png" "CopyAlert_Logo.png"
    Write-Host "Cropped successfully"
} else {
    $img.Dispose()
    Write-Host "Image is fully transparent or error"
}
